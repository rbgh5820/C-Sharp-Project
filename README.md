C#으로 만든것

1. FFMpeg
  - FFMpeg 인코더,디코더 샘플 코드 구하기
  - FFMpeg H263,H264코덱으로 인코딩 후 파일로 저장하고 파일을 읽어서 디코딩 하기
  - OpenCV 웹캠 화면을 FFMpeg 코덱을 사용하여 UDP소켓으로 전송하기(화면 전송) 
2. UDP 소켓으로 이미지 송,수신
3. DB연동(windowsForm)
4. OpenCV 웹캠 + UDP 소켓 송,수신
 - OpenCVSharp를 이용해 Form에 캠화면 구현
 - 캠 화면을 UDP 소켓과 Thread를 사용하여 실시간으로 캠화면 송,수신
5. TCP 소켓 송,수신
6. NAudio를 이용한 음성 출력
  - 녹음 기능을 만들고 플레이하기 (NAudio waveIn,waveOut)
  - 음성 코덱을 사용하여 오디오 인코딩(녹화), 디코딩(실행) 파일을 생성하기 (디코딩 할 시 인코딩 파일을 Read하여 실행)
  - 파일을 생성하지않고 녹음한것을 실시간으로 인코딩하여 UDP소켓으로 송,수신(디코딩에서 Play)
