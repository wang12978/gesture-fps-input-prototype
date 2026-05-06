import cv2
import numpy as np
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
import socket
import time

MODEL_PATH = "hand_landmarker.task"


def is_finger_folded(tip_id, pip_id, landmarks, img_h):
    tip_y = landmarks[tip_id].y * img_h
    pip_y = landmarks[pip_id].y * img_h
    return tip_y > pip_y


def is_fist(landmarks, img_h):
    index_folded = is_finger_folded(8, 6, landmarks, img_h)
    middle_folded = is_finger_folded(12, 10, landmarks, img_h)
    ring_folded = is_finger_folded(16, 14, landmarks, img_h)
    pinky_folded = is_finger_folded(20, 18, landmarks, img_h)
    return index_folded and middle_folded and ring_folded and pinky_folded


def draw_landmarks(frame, hand_landmarks_list):
    for hand_landmarks in hand_landmarks_list:
        for lm in hand_landmarks:
            x = int(lm.x * frame.shape[1])
            y = int(lm.y * frame.shape[0])
            cv2.circle(frame, (x, y), 4, (0, 255, 0), -1)


def main():
    shoot_count = 0

    base_options = python.BaseOptions(model_asset_path=MODEL_PATH)
    options = vision.HandLandmarkerOptions(
        base_options=base_options,
        num_hands=1,
        min_hand_detection_confidence=0.7,
        min_hand_presence_confidence=0.7,
        min_tracking_confidence=0.7,
    )

    detector = vision.HandLandmarker.create_from_options(options)




    udp_ip = "127.0.0.1"
    udp_port = 5055
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    last_shoot_time = 0
    shoot_cooldown = 0.3  # Seconds, prevent crazy continuous triggering




    cap = cv2.VideoCapture(0)
    if not cap.isOpened():
        print("The camera cannot be opened.")
        return

    while True:
        ret, frame = cap.read()
        if not ret:
            print("The camera image cannot be read")
            break

        frame = cv2.flip(frame, 1)
        img_h, img_w, _ = frame.shape

        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb_frame)

        result = detector.detect(mp_image)

        gesture_text = "NO GESTURE"

        if result.hand_landmarks:
            landmarks = result.hand_landmarks[0]

            # Palm center: use wrist as a simplified control point
            palm_x = landmarks[0].x
            palm_y = landmarks[0].y

            # The center of the picture is 0.5, 0.5
            scale = 2.0
            look_x = (palm_x - 0.5) * scale
            look_y = (palm_y - 0.5) * scale

            look_x = max(min(look_x, 1), -1)
            look_y = max(min(look_y, 1), -1)

            # Dead zone, avoid slight hand tremors that may cause the field of view to move randomly
            deadzone = 0.08

            if abs(look_x) < deadzone:
                look_x = 0

            if abs(look_y) < deadzone:
                look_y = 0

            # If it's not a clenched fist, send the perspective control
            if not is_fist(landmarks, img_h):
                message = f"LOOK:{look_x}:{look_y}"
                sock.sendto(message.encode(), (udp_ip, udp_port))
            else:
                gesture_text = "SHOOT"

                current_time = time.time()
                if current_time - last_shoot_time > shoot_cooldown:
                    sock.sendto(b"SHOOT", (udp_ip, udp_port))
                    shoot_count += 1

                    print(f"[SHOOT] count: {shoot_count}")
                    last_shoot_time = current_time

        color = (0, 0, 255) if gesture_text == "SHOOT" else (255, 255, 255)
        cv2.putText(
            frame,
            gesture_text,
            (30, 50),
            cv2.FONT_HERSHEY_SIMPLEX,
            1.2,
            color,
            3
        )

        cv2.imshow("Gesture Recognition", frame)

        key = cv2.waitKey(1) & 0xFF
        if key == 27:  # ESC
            break

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()