using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Tiny;

using Unity.Tiny.Input;

public class GameManagerSystem : ComponentSystem
{
    public static GameManagerSystem Instance;

    public enum Gamestate { spawn,firstCard,secondCard,check,wrong,right,finish};

    public Gamestate myGameState;

    public Entity firstCardSelected;
    public Entity secondCardSelected;

    float checkWaitTimer = 0f;
    float restartWaitTimer = 0f;
    int rightCount = 0;

    public bool isWin = false;

    protected override void OnStartRunning()
    {
        Instance = this;
    }

    protected override void OnCreate()
    {
        myGameState = Gamestate.spawn;
        Entity e = EntityManager.CreateEntity();
        EntityManager.AddComponentData(e, new GameState { Value = GameStates.None });        
    }

    protected override void OnUpdate()
    {
        var gameState = GetSingleton<GameState>();
        //var winPanel = GetSingleton<WinPanel>();

        var input = World.GetExistingSystem<InputSystem>();
        //if (input.GetKeyDown(KeyCode.Space))
        //{
        //    isWin = !isWin;
        //    if (isWin)
        //        gameState.Value = GameStates.Win;
        //    else
        //        gameState.Value = GameStates.None;

        //    SetSingleton(gameState);
        //}

        //if (isWin)
        //{
        //    EntityManager.SetEnabled(winPanel.entity, true);
        //}
        //else
        //{
        //    EntityManager.SetEnabled(winPanel.entity, false);
        //}
        

        switch (myGameState)
        {
            case Gamestate.spawn:
                SpawnCardSystem.Instance.Spawn();
                myGameState = Gamestate.firstCard;
                break;
            case Gamestate.firstCard:
                if(firstCardSelected != Entity.Null)
                {                    
                    EntityManager.SetComponentData(firstCardSelected, new Rotation { Value = quaternion.EulerXYZ(math.radians(0),math.radians(180),math.radians(0)) });                    
                    myGameState = Gamestate.secondCard;
                }
                break;
            case Gamestate.secondCard:
                if (secondCardSelected != Entity.Null)
                {
                    EntityManager.SetComponentData(secondCardSelected, new Rotation { Value = quaternion.EulerXYZ(math.radians(0), math.radians(180), math.radians(0)) });
                    myGameState = Gamestate.check;
                }
                break;
            case Gamestate.check:
                
                int firstId = EntityManager.GetComponentData<CardEntityComponent>(firstCardSelected).id;
                int secondId = EntityManager.GetComponentData<CardEntityComponent>(secondCardSelected).id;
                if (checkWaitTimer < 0.1f)
                {
                    checkWaitTimer += Time.DeltaTime;
                }
                else
                {
                    checkWaitTimer = 0f;

                    if (firstId == secondId)
                        myGameState = Gamestate.right;
                    else
                        myGameState = Gamestate.wrong;
                }
                break;
            case Gamestate.wrong:
                EntityManager.SetComponentData(firstCardSelected, new Rotation { Value = quaternion.identity });
                EntityManager.SetComponentData(secondCardSelected, new Rotation { Value = quaternion.identity });

                firstCardSelected = Entity.Null;
                secondCardSelected = Entity.Null;

                myGameState = Gamestate.firstCard;
                break;
            case Gamestate.right:
                rightCount++;
                if (rightCount >= 8)
                    myGameState = Gamestate.finish;
                else
                {                    
                    firstCardSelected = Entity.Null;
                    secondCardSelected = Entity.Null;

                    myGameState = Gamestate.firstCard;
                }

                break;
            case Gamestate.finish:
                Debug.Log("You win");
                isWin = true;
                gameState.Value = GameStates.Win;
                SetSingleton(gameState);

                if (restartWaitTimer < 2f)
                {
                    restartWaitTimer += Time.DeltaTime;
                }
                else
                {
                    restartWaitTimer = 0f;

                    firstCardSelected = Entity.Null;
                    secondCardSelected = Entity.Null;
                    rightCount = 0;

                    myGameState = Gamestate.spawn;
                }
                break;
        }        
    }


    public void SetGameState(Gamestate state)
    {
        myGameState = state;
    }

    public void SetFirstCardSelected(Entity e)
    {
        firstCardSelected = e;
    }

    public void SetSecondCardSelected(Entity e)
    {
        secondCardSelected = e;
    }
}
