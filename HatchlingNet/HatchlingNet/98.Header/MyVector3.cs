namespace HatchlingNet
{
    public struct MyVector3
    {
        public float x;
        public float y;
        public float z;

        public UnityEngine.Vector3 Vector
        {
            get
            {
                return new UnityEngine.Vector3(x, y, z);
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }
    }
}
