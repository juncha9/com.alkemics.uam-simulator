namespace UAM
{
    public class EVTOL_Mover : Mover
    {
        public float targetKnotPHour
        {
            set => targetSpeed = value * UAMStatic.speed2KnotPHour;
            get => targetSpeed * UAMStatic.knotPHour2Speed;
        }
    }


}
