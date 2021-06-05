# ProjectFVM - 날아다니는 자판기 프로젝트
## 일정 안내
**<프로토타입 기초 개발> [~6/13]**

- **프로젝트 세팅 (6/4 ~ 6/6)**

    - [x] 기본 유니티 3D 프로젝트 구성

    - [x] 복셀 관련 및 필요한 에셋 스토어 자료 임포트

    - [x] 가능하면 Git 세팅까지 [x]

- **동전 수집 게임 (6/11 ~ 6/12)**

플레이어와 맵, 코인은 더미 사용
플레이어 상하좌우 스와이프 이동 구현
장애물 충돌과 언덕 올라가는 관련 처리 구현
코인 획득 구현

- 미구현 : 스테이지 구성 관련 (난이도, 테마 등)
- **자판기 발사 게임 (6/13, 6/18 ~ 6/20)**
- 자판기와 상품, 배경, 장애물은 더미 사용
- 자판기 구성 단계 구현

     자판기 상품 배열, 구매 다이얼 등

- 자판기 날아가는 단계 구현

    연료, 무게, 출력의 기본 구성

    자판기의 좌, 우 Yaw 조작 구현

- 미구현 : 업그레이드 요소

## 프로젝트 세팅 안내
**프로젝트에 사용되는 유니티 버전 : 2021.1.10f1**

(자세한 사항은 **[FVM\ProjectSettings\ProjectVersion]** 에서 확인)



**<유니티 및 프로젝트 파일 임포트 세팅 방법>**
1. **[Unity Hub](https://unity3d.com/kr/get-unity/download "Unity Download")** 를 설치하여 실행
2. **[설치]** 메뉴에서 **[추가]** 를 눌러서 **[정식 릴리스]** 또는 **[Unity 다운로드 아카이브](https://unity3d.com/get-unity/download/archive "Unity Download Archive")** 페이지를 통해서 프로젝트에 사용되는 버전으로 다운로드
3. 설치가 끝나면 **[프로젝트]** 메뉴에서 **[추가]** 를 눌러서 프로젝트 파일 폴더를 선택
4. 프로젝트가 정상적으로 열린다면 성공, 끝!
