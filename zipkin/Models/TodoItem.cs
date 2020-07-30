namespace zipkin.Models
{
    public class TodoItem
    {
        public long id {get;set;}
        public string name {get;set;}
        public bool isComplete {get;set;}

        public override string ToString() {
            return $@"
            TodoItem:
              Id:           {id}
              Name:         {name}
              IsCompleted:  {isComplete}
            ";
        }
    }
}