using Common;
using Google.Protobuf.Reflection;
using System.CodeDom.Compiler;
using System.Text;

namespace ProtocolTool
{
    public class FiledIndexCompare : IComparer<FieldDescriptorProto>
    {
        public int Compare(FieldDescriptorProto? left, FieldDescriptorProto? right)
        {
            ArgumentNullException.ThrowIfNull(left, nameof(left));
            ArgumentNullException.ThrowIfNull(right, nameof(right));

            return left.Number.CompareTo(right.Number);
        }
    }

    public class ProtoData
    {
        public string Name = string.Empty;
        public string Return = string.Empty;
        public SortedSet<FieldDescriptorProto> Parameters = new();
        public string FieldName = string.Empty;
        public string FieldTypeName = string.Empty;
    }

    public class CodeGenerator
    {
        private string _mainProtoName = string.Empty;
        private string _csPackageName = string.Empty;
        private FileDescriptorProto? _proto;

        public CodeGenerator(string mainProtoName)
        {
            _mainProtoName = mainProtoName;
        }

        public string Generate(string path, string fileName)
        {
            string mainProtoName = "GameProto";
            var ProtoDatas = new List<ProtoData>();

            var set = new FileDescriptorSet();
            set.AddImportPath(path);
            set.Add(fileName, true);
            set.Process();

            _proto = set.Files[0];

            var mainProto = _proto.MessageTypes.Find(e => e.Name == mainProtoName);
            if (mainProto != null)
            {
                foreach (var field in mainProto.Fields)
                {
                    if (field.ShouldSerializeOneofIndex() == true)
                    {
                        var datas = CollectProtoDatas(field);
                        ProtoDatas.AddRange(datas);
                    }
                }
            }

            string packageName = _proto?.Package ?? "";
            var tokens = packageName.Split('.').Select(e => e.Trim()).ToList();
            _csPackageName = string.Join(".", tokens.Select(e => e.FirstCharToUpper()).ToList());

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var writer = new IndentedTextWriter(sw))
            {
                GenerateProtoDatas(writer, ProtoDatas);
            }

            return sb.ToString();
        }

        private void GenerateProtoDatas(IndentedTextWriter writer, List<ProtoData> protoDatas)
        {
            writer.WriteLine("static public class ProtoHelper");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (var data in protoDatas)
            {
                if (data.FieldName.EndsWith("Res") == true)
                {
                    GenerateRes(writer, data);
                }
                else
                {
                    GenerateDefault(writer, data);
                }
            }

            writer.Indent--;
            writer.WriteLine("}");
        }

        private void GenerateRes(IndentedTextWriter writer, ProtoData data)
        {
            string returnName = $"{_csPackageName}.{data.Return}";
            writer.WriteLine($"static public {returnName} {data.Name.FirstCharToUpper()}({ToParamString(data, true)})");
            writer.WriteLine("{");
            writer.Indent++;

            writer.WriteLine($"return new {returnName}");
            writer.WriteLine("{");
            writer.Indent++;

            writer.WriteLine($"{data.FieldName.FirstCharToUpper()} = new {ToCsTypeName(data.FieldTypeName)}");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (var param in data.Parameters)
            {
                if (param.type == FieldDescriptorProto.Type.TypeMessage)
                {
                    writer.WriteLine($"{param.Name.FirstCharToUpper()} = {param.Name} ?? new {ToCsTypeName(param.TypeName)}(),");
                }
                else
                {
                    writer.WriteLine($"{param.Name.FirstCharToUpper()} = {param.Name},");
                }
            }

            writer.Indent--;
            writer.WriteLine("}");

            writer.Indent--;
            writer.WriteLine("};");

            writer.Indent--;
            writer.WriteLine("}");
        }
        
        private void GenerateDefault(IndentedTextWriter writer, ProtoData data)
        {
            string returnName = $"{_csPackageName}.{data.Return}";
            writer.WriteLine($"static public {returnName} {data.Name.FirstCharToUpper()}({ToParamString(data)})");
            writer.WriteLine("{");
            writer.Indent++;

            writer.WriteLine($"return new {returnName}");
            writer.WriteLine("{");
            writer.Indent++;

            writer.WriteLine($"{data.FieldName.FirstCharToUpper()} = new {ToCsTypeName(data.FieldTypeName)}");
            writer.WriteLine("{");
            writer.Indent++;

            foreach (var param in data.Parameters)
            {
                writer.WriteLine($"{param.Name.FirstCharToUpper()} = {param.Name},");
            }

            writer.Indent--;
            writer.WriteLine("}");

            writer.Indent--;
            writer.WriteLine("};");

            writer.Indent--;
            writer.WriteLine("}");
        }

        private string ToCsTypeName(string typeName)
        {
            string packageName = _proto?.Package ?? "";
            string typeNameWithoutPackage = typeName.Replace($".{packageName}", "");

            var tokens = typeNameWithoutPackage.Split('.').Select(e => e.Trim()).ToList();
            tokens.RemoveAt(0);

            return $"{_csPackageName}.{string.Join(".Types.", tokens)}";
        }

        private string ToParamString(ProtoData data, bool nullableMessage = false)
        {
            var csParams = new List<string>();
            foreach (var param in data.Parameters)
            {
                switch (param.type)
                {
                    case FieldDescriptorProto.Type.TypeDouble:
                        break;
                    case FieldDescriptorProto.Type.TypeFloat:
                        break;
                    case FieldDescriptorProto.Type.TypeInt64:
                        break;
                    case FieldDescriptorProto.Type.TypeUint64:
                        break;
                    case FieldDescriptorProto.Type.TypeInt32:
                        break;
                    case FieldDescriptorProto.Type.TypeFixed64:
                        break;
                    case FieldDescriptorProto.Type.TypeFixed32:
                        break;
                    case FieldDescriptorProto.Type.TypeBool:
                        break;
                    case FieldDescriptorProto.Type.TypeString:
                        break;
                    case FieldDescriptorProto.Type.TypeGroup:
                        break;
                    case FieldDescriptorProto.Type.TypeMessage:
                        {
                            if (nullableMessage == true)
                            {
                                string csParam = $"{ToCsTypeName(param.TypeName)}? {param.Name} = null";
                                csParams.Add(csParam);
                            }
                            else
                            {
                                string csParam = $"{ToCsTypeName(param.TypeName)} {param.Name}";
                                csParams.Add(csParam);
                            }
                        }
                        break;
                    case FieldDescriptorProto.Type.TypeBytes:
                        break;
                    case FieldDescriptorProto.Type.TypeUint32:
                        break;
                    case FieldDescriptorProto.Type.TypeEnum:
                        {
                            string csParam = $"{ToCsTypeName(param.TypeName)} {param.Name}";
                            csParams.Add(csParam);
                        }
                        break;
                    case FieldDescriptorProto.Type.TypeSfixed32:
                        break;
                    case FieldDescriptorProto.Type.TypeSfixed64:
                        break;
                    case FieldDescriptorProto.Type.TypeSint32:
                        break;
                    case FieldDescriptorProto.Type.TypeSint64:
                        break;
                }
            }

            return string.Join(", ", csParams);
        }

        private List<ProtoData> CollectProtoDatas(FieldDescriptorProto field)
        {
            var fieldDescriptor = FindFieldType(field.TypeName);
            if (fieldDescriptor == null)
                return new List<ProtoData>();

            // oneof 필드는 하나만 있는 것으로 가정한다.

            var normals = new List<FieldDescriptorProto>();
            var oneofs = new List<FieldDescriptorProto>();
            foreach (var child in fieldDescriptor.Fields)
            {
                if (child.ShouldSerializeOneofIndex() == true)
                {
                    oneofs.Add(child);
                }
                else
                {
                    normals.Add(child);
                }
            }

            var protoDatas = new List<ProtoData>();
            foreach (var message in oneofs)
            {
                var parameters = new SortedSet<FieldDescriptorProto>(new FiledIndexCompare());
                foreach (var normal in normals)
                {
                    parameters.Add(normal);
                }

                parameters.Add(message);

                protoDatas.Add(new ProtoData
                {
                    Name = message.Name,
                    Return = _mainProtoName,
                    Parameters = parameters,
                    FieldName = field.Name,
                    FieldTypeName = field.TypeName,
                });
            }

            return protoDatas;
        }

        private DescriptorProto? FindFieldType(string typeName)
        {
            if (_proto == null)
                return null;

            var tokens = typeName.Split('.').Select(e => e.Trim()).ToList();
            tokens.RemoveAt(0);

            DescriptorProto? desc = null;
            foreach (string token in tokens)
            {
                if (desc == null)
                {
                    desc = _proto.MessageTypes.Find(e => e.Name == token);
                }
                else
                {
                    desc = desc.NestedTypes.Find(e => e.Name == token);
                }
            }

            return desc;
        }
    }
}
