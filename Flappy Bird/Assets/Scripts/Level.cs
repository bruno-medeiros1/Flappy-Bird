using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;//tamanho da nossa camera ortografica para sabermos como spawnar os pipes

    private const float PIPE_WIDTH = 7.8f; //largura do nosso cano principal
    private const float PIPEHEAD_HEIGHT = 3.8f;// altura da head cano
    private const float PIPE_MOVE_SPEED = 25f;//velocidade do PIPE
    private const float PIPE_DESTROY_X_POS = -40f;//Posicao do x onde os pipes tem de ser destruidos
    private const float PIPE_SPAWN_X_POS = +40f;//Posicao do x onde os pipes tem de ser spawnados

    private const float CLOUD_WIDTH = 49f;
    private const float CLOUD_HEIGHT = 34f;
    private const float CLOUD_DESTROY_X_POS = -50f;

    private const float GROUND_DESTROY_X_POS = -60f;
    private const float GROUND_WIDTH = 48f;
    private const float GROUND_HEIGHT = -48.6f;

    private const float BIRD_X_POS = 0f;
    
    private int Score;

    /*Variaveis de tempo para ser Spawnado o Pipe*/
    private float PipeSpawnTimer;
    private float PipeSpawnTimerMax;

    private float GapSize; //Variavel usada para quando spawnar os pipes
    private int PipesSpawned;//variavel de referencia para definirmos a dificuldade no nosso jogo

    private static Level instance;//instancia para podermos aceder a esta classe

    private Game_Status state;

    /*criação de uma lista que guarde as posiçoes dos tipos pipes que são spawnados
    Para isso usamos a classe pipe que contem essas informações*/
    private List<Pipe> PipeList;

    private List<Transform> GroundList;

    private List<Transform> CloudList;
    private float CloudTimer;

    private void Awake()
    {
        instance = this;//definimos a nossa instancia
        //Inicializamos a nossa lista
        PipeList = new List<Pipe>();
        SpawnInitialGround();
        SpawnInitialClouds();
        PipeSpawnTimerMax = 1f; //spawna um pipe a cada segundo
        SetDifficulty(Difficulty.Easy);
        state = Game_Status.WaitingToStart;//definimos o estado do jogo
    }

    private void Start()
    {
        CreateGapPipes(50, 20, 12);
        //CreatePip(50, -12, true);
        //subscribe to the event on Bird Class
        Bird.GetInstance().OnDied += Level_onDied;
        Bird.GetInstance().OnStartedPlaying += Level_onStart;
    }

    private void Level_onStart(object sender, EventArgs e)
    {
        state = Game_Status.Playing;
    }

    private void Level_onDied(object sender, EventArgs e)
    {
        state = Game_Status.BirdIsDead;        
    }

    private void Update()
    {
        /*Se o Passaro nao tiver morrido continua a spawnar pipes*/
        if(state == Game_Status.Playing) 
        {
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGround();
            HandleClouds();
        }
        
    }

    private enum Game_Status 
    {
        WaitingToStart,
        Playing,
        BirdIsDead,
    }
    public enum Difficulty 
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }
    private void SetDifficulty(Difficulty difficulty) 
    {
        switch (difficulty) 
        {
            case Difficulty.Easy:
                GapSize = 50f;
                PipeSpawnTimerMax = 1.2f;
                break;
            case Difficulty.Medium:
                GapSize = 40f;
                PipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Hard:
                GapSize = 35f;
                PipeSpawnTimerMax = 1.0f;
                break;
            case Difficulty.Impossible:
                PipeSpawnTimerMax = .9f;
                GapSize = 28f;
                break;
        }
    }
    private Difficulty GetDifficulty() 
    {
        if (PipesSpawned >= 50) return Difficulty.Impossible;
        if (PipesSpawned >= 30) return Difficulty.Hard;
        if (PipesSpawned >= 15) return Difficulty.Medium;
        return Difficulty.Easy;
        
    }
    
    private void SpawnInitialGround()
    {
        GroundList = new List<Transform>();
        Transform groundTr;
        groundTr = Instantiate(GameAssets.GetInstance().Ground_Pref, new Vector3(0, GROUND_HEIGHT, 0), Quaternion.identity);
        GroundList.Add(groundTr);
        groundTr = Instantiate(GameAssets.GetInstance().Ground_Pref, new Vector3(GROUND_WIDTH, GROUND_HEIGHT, 0), Quaternion.identity);
        GroundList.Add(groundTr);
        groundTr = Instantiate(GameAssets.GetInstance().Ground_Pref, new Vector3(GROUND_WIDTH * 2 , GROUND_HEIGHT, 0), Quaternion.identity);
        GroundList.Add(groundTr);
    }
    private void HandleGround() 
    {
        foreach(Transform groundTr in GroundList)
        {
            groundTr.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;


            if(groundTr.position.x < GROUND_DESTROY_X_POS) 
            {
                /*Passou o lado esquerdo realoca do lado direito*/
                float rightmostPos = -100f;
                for(int i = 0; i < GroundList.Count; i++) 
                {
                    if(GroundList[i].position.x > rightmostPos) 
                    {
                        rightmostPos = GroundList[i].position.x;
                    }
                }
                groundTr.position = new Vector3(rightmostPos + GROUND_WIDTH, groundTr.position.y, groundTr.position.z);
            }

        }
    }
    /*Funcao que nos devolve um Transform de um dos 3 Prefabs das clouds
     para assim ser random o spawning das nunvens*/
    private Transform GetRandomCloud_Pref()
    {
        switch (UnityEngine.Random.Range(0, 3))
        {
            default:
            case 1:
                return GameAssets.GetInstance().Cloud1_Pref;
            case 2:
                return GameAssets.GetInstance().Cloud2_Pref;
            case 3:
                return GameAssets.GetInstance().Cloud3_Pref;
        }
    }

    /*Função que spawna as nuvens iniciais*/
    private void SpawnInitialClouds() 
    {
        CloudList = new List<Transform>();//inicializamos a lista
        Transform cloudTR;
        cloudTR = Instantiate(GetRandomCloud_Pref(), new Vector3(0, CLOUD_HEIGHT, 0), Quaternion.identity);
        CloudList.Add(cloudTR);

    }
    private void HandleClouds()
    {
        /*TIMER FOR SPAWNING NEW CLOUD*/
        CloudTimer -= Time.deltaTime;
        if(CloudTimer < 0) 
        {
            float CloudTimerMax = 3f;
            CloudTimer = CloudTimerMax;
            Transform Cloud_TR = Instantiate(GetRandomCloud_Pref(), new Vector3(CLOUD_WIDTH, CLOUD_HEIGHT, 0), Quaternion.identity);
            CloudList.Add(Cloud_TR);
        }
        /*CLOUD MOVEMENT*/
        for(int i = 0; i < CloudList.Count; i++)
        {
            Transform Cloud_TR = CloudList[i];

            /*MOVEMENT BY PARALLAX EFFECT
             As nuvens movimentam se mais devagar que o resto*/
            Cloud_TR.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * .5f;

            if(Cloud_TR.position.x < CLOUD_DESTROY_X_POS)
            {
                Destroy(Cloud_TR.gameObject);
                CloudList.RemoveAt(i);
                i--;
            }
        }

    }

    private void HandlePipeSpawning() 
    {
        /*Time.deltatime devolve o tempo que demorou a Engine a processar o ultimo frame*/
        PipeSpawnTimer -= Time.deltaTime;
        if(PipeSpawnTimer < 0) 
        {
            PipeSpawnTimer += PipeSpawnTimerMax;//resetamos o nosso relogio

            float heightEdgeLimit = 10f;//variavel que previne que o gap nao seja no bottom ou em cima
            float min_height = GapSize * 0.5f + heightEdgeLimit;

            float max_height = CAMERA_ORTHO_SIZE * 2 - GapSize * 0.5f - heightEdgeLimit;

            float height = UnityEngine.Random.Range(min_height, max_height); 

            CreateGapPipes(height, GapSize, PIPE_SPAWN_X_POS);
        }
    }
    private void HandlePipeMovement()
    {
        /*Para cada objecto da Classe Pipe 
         vamos movimenta-lo*/
        for(int i=0; i < PipeList.Count; i++)
        {
            Pipe pipe = PipeList[i];
            /*Se o pipe estiver à direita do jogador não recebe score pois ainda nao passou o obstaculo*/
            bool IsToTheRightOfBird = pipe.GetX() > BIRD_X_POS;
            pipe.Move();

            if (IsToTheRightOfBird && BIRD_X_POS >= pipe.GetX() && pipe.GetBottom()) 
            {
                Score++;
                SoundManager.GetInstance().Play("Score");
            }
            /*Se isto acontecer é pq queremos destroir o nosso objecto*/
            if(pipe.GetX() < PIPE_DESTROY_X_POS) 
            {
                pipe.destroy();
                /*Remoção na lista do Pipe que destruimos*/
                PipeList.Remove(pipe);
                i--;
            }
        }
    }
    /*Função responsavel por criar os dois pipes com um espaço que quisermos (GapSize)*/
    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        /*BOTTOM PIPE*/
        CreatePip(gapY - (gapSize *0.5f), xPosition, true);
        /*UP PIPE*/
        CreatePip((CAMERA_ORTHO_SIZE * 2) - gapY - (gapSize *0.5f), xPosition, false);

        PipesSpawned++;

        SetDifficulty(GetDifficulty());
    }

    /*Função responsável por criar os nossos Pipes(Obstaculos) com uma certa altura, posição no x
     e em cima ou em baixo*/
    private void CreatePip(float height, float xPos, bool CreateBottom) 
   {
        
        /*Inicializamos as variaveis que guardam a posição do pipeHead e pipeBody que se encontram na classe GameAssets*/
        Transform PipeHead = Instantiate(GameAssets.GetInstance().PipeHead_Pref);

        float PipeHeadYPos;//variavel responsavel por guardar a posicao no y do nosso sprite PipeHead
        float offset = 0.1f;

        if (CreateBottom) 
        {
            /*Vamos criar em baixo se estiver true*/
            PipeHeadYPos = -CAMERA_ORTHO_SIZE + height + (PIPEHEAD_HEIGHT * 0.5f - offset);
        }
        else 
        {
            /*Vamos criar em cima se estiver falso*/
            PipeHeadYPos = +CAMERA_ORTHO_SIZE - height - (PIPEHEAD_HEIGHT * 0.5f - offset);
        }

        /*A pipeHead vai ter de ser posicionada da seguinte forma:
         *Dependendo da orientação do pipe se é bottom ou upwards
         ao comprimento total do cano principal somamos ou subtraimos por metade
        do comprimento da cabeça do pipe (PIPE_HEAD)*/
        PipeHead.position = new Vector3(xPos, PipeHeadYPos);
        

        /*********************CANO PRINCIPAL******************/
        Transform PipeBody = Instantiate(GameAssets.GetInstance().Pipe_Pref);
        float PipeBodyYPos;
        if (CreateBottom)
        {
            /*Vamos criar no bottom se estiver true*/
            PipeBodyYPos = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            /*Vamos criar em cima se estiver falso*/
            PipeBodyYPos = +CAMERA_ORTHO_SIZE;
            /*Como o pivot esta definido no bottom temos de invertir 
             para o pipe não sair do screen*/
            PipeBody.localScale = new Vector3(1, -1, 1);
        }
        PipeBody.position = new Vector3(xPos, PipeBodyYPos);
        

        SpriteRenderer pipeBodySpriteRenderer = PipeBody.GetComponent<SpriteRenderer>();
        /*Estamos a definir a altura do PipeBody no DrawMode -> Size do SpriteRenderer do PipeBody*/
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        /*Definimos que o BoxCollider tenha a mesma altura que o PipeBody e a mesma Width*/
        PipeBody.GetComponent<BoxCollider2D>().size = new Vector2(PIPE_WIDTH, height);
        /*Uma vez que o pivot do spite do PipeBody se encontra no bottom o box collider vai ter como origem 
         aquele pivot por isso que não vai ficar direito o box collider, para contornar este problema adicionamos
        um offset de metade da altura para assim compensar.*/
        PipeBody.GetComponent<BoxCollider2D>().offset = new Vector2(0f, height / 2);



        Pipe pipe_obj = new Pipe(PipeHead, PipeBody, CreateBottom);
        PipeList.Add(pipe_obj);
    }



    /*Funcao que devolve quantos Objectos foram spawnados*/
    public int GetPipesSpawned() 
    {
        return PipesSpawned;
    }
    /*Função que devolve o Score*/
    public int GetScore() 
    {
        return Score;
    }
    /*Funcao static para ser acedida por outros scripts*/
    public static Level GetInstance()
    {
        return instance;
    }

    /*Classe que representa o conjunto dos Pipes tanto o Body como a Head*/
    private class Pipe
    {
        private Transform PipeHead_Tr;
        private Transform PipeBody_Tr;
        private bool IsBottom;//variavel auxiliar para o score

        /*Constructor method*/
        public Pipe(Transform PipeHead, Transform PipeBody, bool isBottom) 
        {
            this.PipeHead_Tr = PipeHead;
            this.PipeBody_Tr = PipeBody;
            this.IsBottom = isBottom;
        }
        public void Move() 
        {
            PipeHead_Tr.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            PipeBody_Tr.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }
        /*Getter Method que retorna a posicao do x de um pipe*/
        public float GetX() 
        {
            return PipeBody_Tr.position.x;
        }
        public bool GetBottom() 
        {
            return IsBottom;
        }
        /*Função que destroy o nosso objecto da classe*/
        public void destroy() 
        {
            Destroy(PipeBody_Tr.gameObject);
            Destroy(PipeHead_Tr.gameObject);
        }
    }
}
