using UnityEngine;

public class CarDetail : Item
{
    [SerializeField] CarSegment carSegment;
    public CarSegment GetSegmentType() => carSegment;
}
