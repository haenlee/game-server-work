$protoc = "..\..\Externals\protoc-3.18.0-win64\bin\protoc.exe"
$csharp_out  = "../../GameServer/Proto"
$proto_dir = "../Proto"

$files = Get-ChildItem -Path $proto_dir
foreach($file in $files) {
	write-host $file
	& $protoc --csharp_out=$csharp_out --proto_path=$proto_dir $file
}


