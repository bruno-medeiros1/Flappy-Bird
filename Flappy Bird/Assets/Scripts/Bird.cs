using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JUMP_SPEED = 100f;

    /*Definição de Eventos*/
    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private Rigidbody2D rigidbody;
    private Touch touch;

    /*Instancia da Classe */
    private static Bird instance;

    private State state;
    private enum State 
    {
        WaitingToPlay,
        Playing,
        Dead,
    }
    private void Awake()
    {
        instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToPlay;

    }
    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        switch (state)
        {
            case State.WaitingToPlay:
                /*O jogo vai esperar até que o jogador pressione no ecra para saltar só
                 ai é que começa*/
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    state = State.Playing;
                    rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    /*Inicializamos o Evento*/
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:

                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Jump();
                    
                }
                //permite fazer com que o passaro va cair se nao saltarmos
                transform.eulerAngles = new Vector3(0, 0, rigidbody.velocity.y * .1f);

                break;
            case State.Dead:
                
                break;
        }
#endif
        /**************MOBILE CONTROLLERS************/

        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            switch (state)
            {
                case State.WaitingToPlay:
                    /*O jogo vai esperar até que o jogador pressione no ecra para saltar só
                     ai é que começa*/
                    if (touch.phase == TouchPhase.Began)
                    {
                        state = State.Playing;
                        rigidbody.bodyType = RigidbodyType2D.Dynamic;
                        Jump();
                        /*Inicializamos o Evento*/
                        if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                    }
                    break;
                case State.Playing:

                    if (touch.phase == TouchPhase.Began)
                    {
                        Jump();
                    }
                    //permite fazer com que o passaro va cair se nao saltarmos
                    transform.eulerAngles = new Vector3(0, 0, rigidbody.velocity.y * .2f);

                    break;
                case State.Dead:

                    break;
            }
        }
        
    }

    /*Função responsável pelo Salto do Bird*/
    private void Jump() 
    {
        rigidbody.velocity = Vector2.up * JUMP_SPEED;
        SoundManager.GetInstance().Play("Jump");
    }
    /*Quando o jogador */
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        rigidbody.bodyType = RigidbodyType2D.Static;//para o jogador parar
        SoundManager.GetInstance().Play("Die");
        //Ativamos o nosso evento
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }

    public static Bird GetInstance() 
    {
        return instance;
    }
}
