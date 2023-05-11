if __name__ == '__main__':
    from ultralytics import YOLO
    import os
    import torch

    # os.environ["KMP_DUPLICATE_LIB_OK"] = "TRUE"
    torch.cuda.empty_cache()

    # Load the model.
    model = YOLO('yolov8n.pt')

    # Training.
    results = model.train(
        data='Oot.yaml',
        imgsz=640,
        epochs=500,
        batch=24,  # -1 autobatch
        project="MontaOrderVerification",
        name='oot_model',
        device=0,  # GPU
        save_period=-1,  # No saves inbetween training,
        patience=100
    )

# all arguments https://docs.ultralytics.com/modes/train/#arguments
