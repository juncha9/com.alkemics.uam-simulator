namespace UAM
{
    public class EVTOL_Mover : Mover
    {
        public float targetKnotPHour
        {
            set => targetSpeed = value * UAMStatic.knotPHour2Speed;
            get => targetSpeed * UAMStatic.speed2KnotPHour;
        }
    }


}
