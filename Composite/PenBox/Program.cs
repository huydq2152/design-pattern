// Problem Statement
//
// Title: Recursive Hierarchy Traversal for Nested Inventory Structures
//
// Context: You are provided with a hierarchical data structure consisting of two primary entities: Box and Pen.
//     A Box acts as a container that can hold a collection of Pen objects and a collection of sub-boxes (Box objects), creating a recursive tree structure.
//     A Pen is a leaf entity containing specific attributes such as Name and Color.
//     Objective: Implement a solution to traverse a given Box instance, print the Name, Color of every Pen, Name of every Box and both contained within the entire hierarchy.
//     Additionally, the solution must calculate and display the Depth Level of each Pen relative to the root box (where the root level is typically considered depth 0 or 1).


using PenBox;

Box rootBox = new("Root Box");
rootBox.Add(new Pen("Pen 1", "Red"));

Box box1 = new("Box 1");
box1.Add(new Pen("Pen 2", "Blue"));
box1.Add(new Pen("Pen 3", "Green"));
rootBox.Add(box1);

Box box2 = new("Box 2");
box2.Add(new Pen("Pen 4", "Yellow"));
box1.Add(box2);

Box box3 = new("Box 3");
box2.Add(box3);

rootBox.PrintDetails(0);
