COMPILER       =dotnet
PROJECTS_FLAGS =
MAIN_PROJECT   =src/OnlineShop

build:
	$(COMPILER) build

run:
	$(COMPILER) run --project $(MAIN_PROJECT)

seed:
	$(COMPILER) run --project $(MAIN_PROJECT) -- --seed

create:
	$(COMPILER) run --project $(MAIN_PROJECT) -- --create

db: create seed

all: create seed run
