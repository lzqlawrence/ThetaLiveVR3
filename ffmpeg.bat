rem ffmpegフォルダに移動
cd ffmpeg

rem 現在時刻を取得(先頭が空白の場合は0で置き換え)
set temp=%time: =0%

rem HHMMSS形式にする
set now=%temp:~0,2%%temp:~3,2%%temp:~6,2%

rem 連結用の文字列
set str1=theta
set str3=.mp4

rem 動画の書き出し
ffmpeg -y -f rawvideo -r 10 -vcodec rawvideo -pix_fmt rgba -s 2048x1024 -i \\.\pipe\ThetaLiveVR -vcodec libx264 -pix_fmt yuv420p -s 2048x1024 -b:v 3000k -threads 0 -f mp4 %str1%%now%%str3%