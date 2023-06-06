using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The crosshair and also controller for the mouse<br />
/// Handles crosshair sprite changes, mouse states, and some actions.<br />
/// Ultimately handled by the player in a perfect world.
/// </summary>
public class Crosshair : MonoBehaviour
{
    #region Setup

    #region Public Fields
    //public fields are editable in the unity editor as well as normal instantiations in code
    public Sprite SearchlightCrosshairSprite;
    public Sprite FightCrosshairSprite;
    public Sprite InteractCrosshairSprite;

    public float MouseClampCircleRadius;

    public MouseControlState currentMouseControlState;

    public Vector2 mouseCursorPosition;

    #endregion

    #region Private Fields
    private GameObject player;
    
    private SpriteRenderer crosshairRenderer;

    private UnityEngine.Rendering.Universal.Light2D searchLight;
    private UnityEngine.Rendering.Universal.Light2D flickerLightA;
    private UnityEngine.Rendering.Universal.Light2D flickerLightB;

    private List<MouseControlState> mouseControlStates;

    private Vector3 mouseClampCircleCenter;

    #endregion

    #endregion

    // Start is called before the first frame update, initialize here
    private void Start()
    {
        PlayerConfiguration();
        CrosshairConfiguration();
        LightsConfiguration();
    }

    // Update is called once per frame
    public void Update()
    {
        GetMouseState();
        UpdateSprites();
    }

    #region Configuration
    private void PlayerConfiguration()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void CrosshairConfiguration()
    {
        Cursor.visible = false;
        mouseControlStates = GetMouseControlStates();
        currentMouseControlState = mouseControlStates
            .First(x => x.CrosshairState == CrosshairStateEnum.Searchlight);
        crosshairRenderer = GetComponent<SpriteRenderer>();
    }

    private void LightsConfiguration()
    {
        searchLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        flickerLightA = transform.GetChild(0)
            .GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        flickerLightB = transform.GetChild(1)
            .GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    #endregion

    private void UpdateSprites()
    {
        UpdateCursorSprites();//
    }

    #region Mouse and Cursor Frame by Frame Updates
    private void GetMouseState()
    {
        mouseCursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CheckMouseScrollWheelInput();
        UpdateMouseState();
    }

    private void UpdateMouseState()
    {
        transform.position = mouseCursorPosition;
        UpdateControlStateControls();
        ClampMousePositionAroundPlayer();
    }

    private void ClampMousePositionAroundPlayer()
    {
        mouseClampCircleCenter = player.transform.position;
        float mouseDistanceFromCircleCenter = Vector3.Distance(transform.position, mouseClampCircleCenter);

        if (mouseDistanceFromCircleCenter < MouseClampCircleRadius) return;

        Vector3 mousePositionToCircleCenter = transform.position - mouseClampCircleCenter;
        mousePositionToCircleCenter *= MouseClampCircleRadius / mouseDistanceFromCircleCenter;
        transform.position = mouseClampCircleCenter + mousePositionToCircleCenter;
    }

    private void UpdateCursorSprites()
    {
        // FIXME will effect clicking and holding to change crosshair sprite, for now this works 
        crosshairRenderer.sprite = currentMouseControlState.Sprite;

    }

    //FIXME this idea could be better
    private void UpdateControlStateControls()
    {
        switch (currentMouseControlState.CrosshairState)
        {
            case CrosshairStateEnum.Searchlight:
                EnableSearchlight();
                return;

            case CrosshairStateEnum.Interact:
                DisableSearchlight();
                return;

            case CrosshairStateEnum.Fight:
                DisableSearchlight();
                return;
        }
    }

    public void DisableSearchlight()
    {
        searchLight.enabled = false;
        flickerLightA.enabled = false;
        flickerLightB.enabled = false;
    }

    public void EnableSearchlight()
    {
        searchLight.enabled = true;
        flickerLightA.enabled = true;
        flickerLightB.enabled = true;
    }

    #endregion

    #region Mouse and Keyboard Input Frame by Frame Updates

    private void CheckMouseScrollWheelInput()
    {
        var scrolling = Input.mouseScrollDelta.y != 0;
        var scrollingUp = Input.mouseScrollDelta.y > 0;
        var scrollingDown = Input.mouseScrollDelta.y < 0;

        if (!scrolling) return;

        //hack, could just do an array and get the index, but lists are better
        var currentMouseControlStateIndex = mouseControlStates
            .TakeWhile(x => x.CrosshairState != currentMouseControlState.CrosshairState).Count();

        //scroll down
        if (scrollingDown)
        {
            ScrollDown(currentMouseControlStateIndex);
        }

        //scroll up
        if (scrollingUp)
        {
            ScrollUp(currentMouseControlStateIndex);
        }
    }

    private void ScrollDown(int currentMouseControlStateIndex)
    {
        Debug.Log("Scrolling down");
        //check if incrementing mouseStates index by 1 will go over list length, if it does, revert to mouseControlStates[0]
        if (currentMouseControlStateIndex == mouseControlStates.Count - 1)
        {
            currentMouseControlState = mouseControlStates[0];
        }
        else
        {
            //if not, scroll down to next mouse state
            currentMouseControlState = mouseControlStates[currentMouseControlStateIndex + 1];
        }
    }

    private void ScrollUp(int currentMouseControlStateIndex)
    {
        Debug.Log("Scrolling up");
        //check if decrementing mouseStates index by 1 will go below 0, if it does, revert to mouseControlStates[count - 1]
        if (currentMouseControlStateIndex == 0)
        {
            currentMouseControlState = mouseControlStates[mouseControlStates.Count - 1];
        }
        else
        {
            //if not, scroll down to next mouse state
            currentMouseControlState = mouseControlStates[currentMouseControlStateIndex - 1];
        }
    }

    #endregion

    #region State Stores
    private List<MouseControlState> GetMouseControlStates()
    {
        List<MouseControlState> states = new()
        {
            new()
            {
                CrosshairState = CrosshairStateEnum.Searchlight,
                Sprite = SearchlightCrosshairSprite,
                StateName = "Searchlight"
            },
            new()
            {
                CrosshairState = CrosshairStateEnum.Fight,
                Sprite = FightCrosshairSprite,
                StateName = "Fight"
            },
            new()
            {
                CrosshairState = CrosshairStateEnum.Interact,
                Sprite = InteractCrosshairSprite,
                StateName = "Interact"
            }
        };

        return states;
    }

    #endregion

}
