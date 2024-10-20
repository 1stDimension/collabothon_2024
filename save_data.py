import os 
import pathlib
from datasets import load_dataset

from PIL import Image

this = pathlib.Path(".")
img = this / "img"



donut = load_dataset("katanaml-org/invoices-donut-data-v1")
cdip = load_dataset("chainyo/rvl-cdip-invoice")

images = donut.select_columns("image")["train"]["image"]

for idx,val in enumerate(images[:3]):
    val.save(f"{img}/{idx}.jpeg",quality=80, optimize=True, progressive=True)