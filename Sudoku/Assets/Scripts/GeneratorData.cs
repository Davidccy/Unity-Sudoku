﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorData {
    public class B {
        public string Name;
        public int[,] ArrayData;
    }

    public static readonly List<B> bList = new List<B> {
        new B {
            Name = "Test1",
            ArrayData = new int[9, 9] {
                    {6, 9, 3, 8, 5, 2, 7, 4, 1},
                    {2, 5, 1, 6, 4, 7, 8, 9, 3},
                    {8, 4, 7, 3, 1, 9, 5, 2, 6},
                    {9, 8, 6, 4, 3, 1, 2, 5, 7},
                    {1, 7, 5, 2, 8, 6, 9, 3, 4},
                    {3, 2, 4, 7, 9, 5, 1, 6, 8},
                    {4, 3, 2, 9, 7, 8, 6, 1, 5},
                    {5, 6, 8, 1, 2, 3, 4, 7, 9},
                    {7, 1, 9, 5, 6, 4, 3, 8, 2}
            },
        },
        new B {
            Name = "Test2",
            ArrayData = new int[9, 9] {
                    {9, 3, 4, 2, 5, 7, 1, 6, 8},
                    {2, 8, 5, 1, 6, 9, 3, 4, 7},
                    {6, 1, 7, 3, 4, 8, 2, 5, 9},
                    {3, 6, 8, 7, 9, 1, 5, 2, 4},
                    {1, 4, 9, 5, 2, 3, 7, 8, 6},
                    {5, 7, 2, 4, 8, 6, 9, 1, 3},
                    {7, 2, 1, 8, 3, 4, 6, 9, 5},
                    {8, 5, 6, 9, 7, 2, 4, 3, 1},
                    {4, 9, 3, 6, 1, 5, 8, 7, 2}
            },
        }
    };
}
