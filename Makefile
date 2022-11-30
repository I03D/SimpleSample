all:
	mcs base.cs inventory.cs upgrades.cs -o game.exe
	mono game.exe
