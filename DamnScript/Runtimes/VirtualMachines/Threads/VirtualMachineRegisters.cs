using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public struct VirtualMachineRegisters
    {
        public long this[int index]
        {
            get => index switch
            {
                0 => loopIdentifier0,
                1 => loopIdentifier1,
                2 => loopIdentifier2,
                3 => loopIdentifier3,
                _ => throw new IndexOutOfRangeException()
            };
            set
            {
                switch (index)
                {
                    case 0:
                        loopIdentifier0 = value;
                        break;
                    case 1:
                        loopIdentifier1 = value;
                        break;
                    case 2:
                        loopIdentifier2 = value;
                        break;
                    case 3:
                        loopIdentifier3 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        
        public long loopIdentifier0;
        public long loopIdentifier1;
        public long loopIdentifier2;
        public long loopIdentifier3;
    }
}