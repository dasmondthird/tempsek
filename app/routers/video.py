# server/c2_server/app/routers/video.py
from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import List
from .. import schemas, crud
from ..database import SessionLocal, engine

router = APIRouter(
    prefix="/video",
    tags=["video"]
)

# Зависимость для получения сессии БД
def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

@router.get("/", response_model=List[schemas.VideoFrame])
def read_videos(client_id: int, skip: int = 0, limit: int = 10, db: Session = Depends(get_db)):
    videos = crud.get_video_frames(db, client_id=client_id, skip=skip, limit=limit)
    return videos

@router.post("/", response_model=schemas.VideoFrame)
def create_video(video: schemas.VideoFrameCreate, client_id: int, db: Session = Depends(get_db)):
    return crud.create_video_frame(db=db, video_frame=video, client_id=client_id)