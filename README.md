# 소켓 서버 프로그래밍
- 소켓 생성 > Bind > Listen > Accept(연결 수락) > Receive and Send
- 위 흐름을 기반으로 소켓을 통해 통신하는 서버를 구축하고 배포하기 위한 프로젝트입니다.

# 사용 기술
- .NET 7.0.14
- C# 11
- protoc 3.18.0
- Visual Studio 2022

# 특징
- 다양한 언어와 플랫폼 간 호환성
  - gRPC 통신을 구현함으로써 다양한 프레임워크와 통신할 수 있습니다.
- proto 생성에 대한 편의성
  - proto 파일에서 지정한 포맷으로 작성시, Generator가 패킷을 유연하게 관리해주는 ProtoHelper 스크립트를 생성합니다.
  - 실제 패킷을 주고받을 때, ProtoHelper의 메소드를 호출함으로써 gRPC의 프로세스를 간소화하였습니다.
- 기능 확장성
  - 소켓 서버 기능을 확장하고 구축하기 위한 베이스가 됩니다.
  - 개발자는 다른 요구 사항에 맞게 다른 모듈이나 기능을 추가할 수 있습니다.
  
# 목표
- 세션 생성 및 관리
  - 클라이언트 상태에 대한 정보를 세션으로 관리함으로써, 서버 동시 연결 수를 제한하거나 사용자 인증, 사용자별 데이터를 효율적으로 사용할 수 있습니다.
- 더미 클라이언트 생성
  - 실제 기능들을 시뮬레이션 함으로써 제한된 서버 조건에서 확장성 및 안정성을 테스트할 수 있습니다.
  - 서버의 스트레스 테스트를 통해 서버의 리미트를 알 수 있고 과부화된 상황에서 정상적인 복구가 가능한지 확인할 수 있습니다.
- 성능 테스트
  - 다양한 조건에서 서버가 효율적으로 작동하는지 확인할 수 있습니다.
  - 서버의 주요 지표에 대해 모니터링하고 그 결과를 바탕으로 인프라를 구축할 수 있습니다.