namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    internal class MaterialGroup
    {
        private int id;

        private double length;

        private double width;

        private Dictionary<string, int> maxcutcount;

        private List<Material> allMaterials;

        internal MaterialGroup(int id, double length, double width, int size, Dictionary<string, int> maxcutcount)
        {
            this.id = id;
            this.length = length;
            this.width = width;
            this.maxcutcount = maxcutcount;
            allMaterials = new List<Material>();
            for (int i = 0; i < size; i++)
            {
                this.allMaterials.Add(new Material(this.id, Guid.NewGuid().GetHashCode(), this.length, this.width, maxcutcount));
            }
        }

        internal MaterialGroup(MaterialGroup materialGroup)
        {
            this.id = materialGroup.id;
            this.length = materialGroup.length;
            this.width = materialGroup.width;
            this.maxcutcount = materialGroup.maxcutcount;
            this.allMaterials = materialGroup.GetMaterials().Select(x => x.Clone()).ToList();
        }

        internal int GetId()
        {
            return this.id;
        }

        internal double GetLength()
        {
            return this.length;
        }

        internal double GetWidth()
        {
            return this.width;
        }

        internal int GetSize()
        {
            return allMaterials.Count;
        }

        internal Dictionary<string, int> GetMaxCutCount()
        {
            return maxcutcount;
        }

        internal void Remove(int id)
        {
            var find = allMaterials.Find(x => x.GetId() == id);
            if (find != null)
            {
                this.allMaterials.Remove(find);
            }
        }
        internal List<Material> GetMaterials()
        {
            return this.allMaterials;
        }

        internal MaterialGroup Clone()
        {
            return new MaterialGroup(this);
        }
        public override string ToString()
        {
            return $"{this.length}x{this.width}={this.allMaterials.Count}";
        }
    }
}
