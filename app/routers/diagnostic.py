# app/routers/diagnostic.py
from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from datetime import datetime
from .. import schemas, models, crud
from ..database import SessionLocal
from ..dependencies import get_current_user, get_db

router = APIRouter(
    prefix="/diagnostic",
    tags=["diagnostic"]
)

@router.post("/", response_model=schemas.DiagnosticInfo)
def create_diagnostic(info: schemas.DiagnosticInfoCreate, db: Session = Depends(get_db), user: models.User = Depends(get_current_user)):
    client = crud.get_client_by_device_id(db, device_id=info.device_id)
    if not client:
        client = crud.create_client(db, device_id=info.device_id, current_time=info.current_time)
    else:
        client.last_seen = datetime.utcnow()
        db.commit()
    db_info = crud.create_diagnostic_info(db, info, client.id)
    return db_info