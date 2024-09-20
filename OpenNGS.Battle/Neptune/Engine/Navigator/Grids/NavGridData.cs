using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class NavGridData
{
    public int rows;
    public int cols;
    public float size;

    public byte[] grids;
    public NavGridData(int rows,int cols,float size)
    {
        this.rows = rows;
        this.cols = cols;
        this.size = size;
        this.grids = new byte[this.rows * this.cols];
    }

    public NavGridData(byte[] data)
    {
        this.rows = BitConverter.ToInt16(data, 0);
        this.cols = BitConverter.ToInt16(data, 2);
        this.size = BitConverter.ToSingle(data, 4);
        this.grids = new byte[this.rows * this.cols];
        if (data.Length < this.rows * this.cols + 8)
        {
            Debug.LogError("NavGridData Error:invalid data!");
            return;
        }
        Array.Copy(data, 8, this.grids, 0, this.grids.Length);
    }

    public int GetArea(int row,int col)
    {
        return this.grids[row * this.cols + col];
    }
}
