import cv2
import os

video = cv2.VideoCapture(r"D:\dev\my_Project\AutoGame\frames\My_Project.mp4")
output_dir = r"D:\dev\my_Project\AutoGame\frames"
os.makedirs(output_dir, exist_ok=True)
if not video.isOpened():
    print("動画を開けませんでした。パスか形式を確認してください。")
else:
    print("動画を開けました。読み込み開始します。")
i = 0
while True:
    ret, frame = video.read()
    if not ret:
        print("処理終了、出力完了しました")
        break
    cv2.imwrite(f"{output_dir}/frame_{i:05d}.png", frame)
    i += 1