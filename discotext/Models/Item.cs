namespace discotext.Models
{
    public enum ItemType
    {
        Normal,  
        Key,     
        Clothing  
    }
    
    public delegate void DialogueActionHandler();
    
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanTake { get; set; }
        public ItemType Type { get; set; }
        public bool IsOn { get; set; } = true;
        public bool HasTie { get; set; } = true;
        public bool IsWorn { get; set; } = false;
        public Dictionary<string, string> InteractionResponses { get; set; }
        public List<DialogueOption> DialogueOptions { get; set; }
        public bool HasDialogueChoices => DialogueOptions != null && DialogueOptions.Count > 0;
        
        public DialogueActionHandler ExitDialogueHandler { get; set; }
        public DialogueActionHandler LookHandler { get; set; }

        public Item(string name, string description, bool canTake = true, ItemType type = ItemType.Normal)
        {
            Name = name;
            Description = description;
            CanTake = canTake;
            Type = type;
            InteractionResponses = new Dictionary<string, string>();
            DialogueOptions = new List<DialogueOption>();
        }
    }
    
    public class DialogueOption
    {
        public string Text { get; set; }
        public string Response { get; set; }
        public Action Effect { get; set; }
        
        public DialogueOption(string text, string response, Action effect = null)
        {
            Text = text;
            Response = response;
            Effect = effect;
        }
    }
}