using ForestSimulation;

var forest = new Forest();
forest.PlantTree(1,1, "Oak", "Green", new byte[] { /* texture data */ });
forest.PlantTree(2,2, "Pine", "Dark Green", new byte[] { /* texture data */ });
forest.PlantTree(3,3, "Oak", "Green", new byte[] { /* texture data */ }); // Reuses the Oak Green TreeType

forest.Draw();