using Unity.Entities;

public class SelectCardSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<CardEntityComponent>().ForEach((ref Tappable tappable, ref CardEntityComponent cardEntityComponent) =>
        {
            if (tappable.IsTapped)
            {
                tappable.IsTapped = false;

                if (GameManagerSystem.Instance.myGameState == GameManagerSystem.Gamestate.firstCard)
                {
                    GameManagerSystem.Instance.SetFirstCardSelected(cardEntityComponent.entity);
                }
                else if (GameManagerSystem.Instance.myGameState == GameManagerSystem.Gamestate.secondCard)
                {
                    if (cardEntityComponent.entity != GameManagerSystem.Instance.firstCardSelected)
                    {
                        GameManagerSystem.Instance.SetSecondCardSelected(cardEntityComponent.entity);
                    }
                }
            }
        });
    }
}
