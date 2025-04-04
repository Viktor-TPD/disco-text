using discotext.Models;
using discotext.Utils;

namespace discotext.Core
{
    public class Game
    {
        private Player _player;
        private bool _isRunning;
        private Dictionary<string, Location> _locations;
        private Dictionary<string, Item> _items;
        private CommandProcessor _commandProcessor;
        private GameText _gameText;
        private DialogueHandler _dialogueHandler;

        public Game()
        {
            _gameText = new GameText();
            _dialogueHandler = new DialogueHandler(_gameText);
            
            InitializeGameWorld();
            
            _commandProcessor = new CommandProcessor(this);
            
            _isRunning = false;
        }
        
        private void InitializeGameWorld()
        {
            _items = new ItemFactory(null, _gameText).CreateAllItems();
            
            _locations = new LocationFactory(_items).CreateLocations();
            
            _player = new Player(_locations["CenterOfRoom"]);
            
            _items = new ItemFactory(_player, _gameText).CreateAllItems();
            
            _locations = new LocationFactory(_items).CreateLocations();
            
            _player.CurrentLocation = _locations["CenterOfRoom"];
        }

        public void Start()
        {
            _isRunning = true;
            _gameText.DisplayIntro();
            _gameText.DisplayLocationDescription(_player.CurrentLocation);

            while (_isRunning)
            {
                Console.Write("> ");
                string input = Console.ReadLine().ToLower();

                if (input == "quit" || input == "exit")
                {
                    _isRunning = false;
                    continue;
                }

                _commandProcessor.ProcessCommand(input);
            }

            _gameText.DisplayOutro();
        }

        public Player GetPlayer()
        {
            return _player;
        }

        public GameText GetGameText()
        {
            return _gameText;
        }
        
        public DialogueHandler GetDialogueHandler()
        {
            return _dialogueHandler;
        }

        public void EndGame()
        {
            _isRunning = false;
        }
    }
}