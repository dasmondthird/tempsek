# app/crud.py
from sqlalchemy.orm import Session
from . import models, schemas
from datetime import datetime

def get_client_by_device_id(db: Session, device_id: str):
    return db.query(models.Client).filter(models.Client.device_id == device_id).first()

def create_client(db: Session, device_id: str, current_time: datetime):
    db_client = models.Client(
        device_id=device_id,
        created_at=current_time,
        last_seen=current_time
    )
    db.add(db_client)
    db.commit()
    db.refresh(db_client)
    return db_client

def create_diagnostic_info(db: Session, info: schemas.DiagnosticInfoCreate, client_id: int):
    db_info = models.DiagnosticInfo(**info.dict(), client_id=client_id)
    db.add(db_info)
    db.commit()
    db.refresh(db_info)
    return db_info

def create_video_frame(db: Session, video_frame: schemas.VideoFrameCreate, client_id: int):
    db_video_frame = models.VideoFrame(**video_frame.dict(), client_id=client_id)
    db.add(db_video_frame)
    db.commit()
    db.refresh(db_video_frame)
    return db_video_frame

def get_video_frames(db: Session, client_id: int, skip: int = 0, limit: int = 10):
    return db.query(models.VideoFrame).filter(models.VideoFrame.client_id == client_id).offset(skip).limit(limit).all()

# Аналогично создайте CRUD функции для других данных
# Аналогично создайте CRUD функции для Screenshot и ProcessInfo
