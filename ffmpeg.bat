rem ffmpeg�t�H���_�Ɉړ�
cd ffmpeg

rem ���ݎ������擾(�擪���󔒂̏ꍇ��0�Œu������)
set temp=%time: =0%

rem HHMMSS�`���ɂ���
set now=%temp:~0,2%%temp:~3,2%%temp:~6,2%

rem �A���p�̕�����
set str1=theta
set str3=.mp4

rem ����̏����o��
ffmpeg -y -f rawvideo -r 10 -vcodec rawvideo -pix_fmt rgba -s 2048x1024 -i \\.\pipe\ThetaLiveVR -vcodec libx264 -pix_fmt yuv420p -s 2048x1024 -b:v 3000k -threads 0 -f mp4 %str1%%now%%str3%