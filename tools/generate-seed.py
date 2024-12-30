import os
from faker import Faker
from nanoid import generate
from random import randint, uniform
from hashlib import sha256

fake = Faker()
NANO_ID_SIZE = 25
USERS_ROLES_COUNT = 3
categories = [
    "Electronics",
    "Clothing",
    "Home & Kitchen",
    "Books",
    "Health & Beauty",
    "Sports & Outdoors",
    "Toys & Games",
    "Automotive",
    "Jewelry",
    "Grocery"
]

dir_path = os.path.join("src", "OnlineShop.Data", "Sql", "Init", "Seed")
os.makedirs(dir_path, exist_ok=True)

def gen_scripts(name: str, script: str) -> None:
    file_path = os.path.join(dir_path, name)

    with open(file_path, "w+") as file:
        file.write("-- This file is auto generated using 'generate-seed.py' script\n")
        file.write(script)

def gen_user() -> str:
    password = "Ad@123"
    return f"""
INSERT INTO [AppUsers] ([Id], [Username], [Role], [PasswordHash])
VALUES(
    '{generate(size=NANO_ID_SIZE)}',
    '{fake.name()}',
    {randint(1, USERS_ROLES_COUNT)},
    '{sha256(password.encode()).hexdigest()}');
"""

def gen_products(category_id: str) -> str:
    return f"""
INSERT INTO [Products] ([Id], [Name], [Quantity], [Cost], [Discount], [Details], [CategoryId])
VALUES(
    '{generate(size=NANO_ID_SIZE)}',
    '{fake.word().capitalize()}',
    {randint(0, 30)},
    {round(uniform(5.0, 500.0), 2)},
    {randint(0, 100)},
    '{fake.sentence()}',
    '{category_id}');
"""

def gen_categories() -> str:
    id = generate(size=NANO_ID_SIZE)
    return f"""
-- Categories '{id}'
INSERT INTO [Categories] ([Id], [Label])
VALUES(
    '{id}',
    '{categories.pop()}');
{"".join([gen_products(id) for _ in range(0, 10)])}
"""

gen_scripts("AppUsers.sql", "".join([gen_user() for _ in range(0, 10)]))
gen_scripts("Categories.sql", "".join([gen_categories() for _ in range(0, len(categories))]))
