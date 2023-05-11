if __name__ == '__main__':

    from ultralytics import YOLO
    import os

    os.environ["KMP_DUPLICATE_LIB_OK"]="TRUE"

    # Load the model.
    model = YOLO('yolov8n.pt')

    # Training.
    results = model.train(
        data='Oot.yaml',
        imgsz=640,
        epochs=150,
        batch=16,
        name='oot_model',
        device=0,
        save_period=-1
    )
