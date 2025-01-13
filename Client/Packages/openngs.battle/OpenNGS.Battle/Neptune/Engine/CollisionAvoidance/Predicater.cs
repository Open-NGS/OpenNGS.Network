using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ObstaclePredicater
{
    public int id;
    public OrcaObstacleStatus status;

    public ObstaclePredicater(int value)
    {
        this.id = value;
    }

    public ObstaclePredicater(OrcaObstacleStatus status)
    {
        this.status = status;
    }

    public bool PredicateID(OrcaObstacle ob)
    {
        return ob.Id == id;
    }
    public bool PredicateStatus(OrcaObstacle ob)
    {
        return ob.Status == status;
    }

}