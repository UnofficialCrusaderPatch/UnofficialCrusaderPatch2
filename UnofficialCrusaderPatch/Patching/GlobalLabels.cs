namespace UCP.Patching
{
    public class GlobalLabels
    {
        private static LabelCollection labels = new LabelCollection();

        public static int GetLabel(string labelName)
        {
            return labels.GetLabel(labelName);
        }

        public static void Add(BinLabel label)
        {
            labels.Add(label);
        }

        public static void Clear()
        {
            labels.Clear();
        }
    }
}