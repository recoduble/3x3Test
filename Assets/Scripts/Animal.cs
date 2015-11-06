using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{
    Animator anim;
    public string ClipName, NewName; // 클립이름 PuzzleManager에서 자동으로 설정함 
    public int Index; // PuzzleManager의 컬럼 번호 (ArrayList에서의 인덱스를 바로 찾아오기 위함)
    public bool isDead = false;

    Vector3 oldPos;

    public GameObject Effect; // 사라질때 나오는 DestroyEffect 프리펩

    Animal target;
    PuzzleManager manager; // PuzzleManager 객체

    void Awake()
    {
        // 캐릭터의 생성 당시에는 벨로시티가 전부 0이기 때문에
        // 3매치가 된 캐릭터들이 착지 전에 삭제되는 문제가 생길 수 있다.
        // 그래서 y축의 벨로시티를 -5f로 설정해서 가속을 준다.
        GetComponent<Rigidbody>().velocity = new Vector3(0f, -5f, 0f);
    }

	void Start()
	{
        anim = GetComponent<Animator>();
        anim.Play(ClipName);
        // 하이어라키의 PuzzleManager를 가져온다.
        manager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
	}

    void Update()
    {
        if (!isDead)
        {
            if (Input.GetMouseButton(0))
            {
                if (manager.TouchedAnimal == null)
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (GetComponent<Rigidbody>().velocity.magnitude <= 0.3f) // 움직이지 않고 있는 캐릭터만 체크
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(pos, Vector3.forward, out hit, 100f))
                        {
                            if (hit.collider.gameObject == gameObject)
                            {
                                manager.TouchedAnimal = gameObject; // 터치한 캐릭터를 저장해둔다.
                            }
                        }
                    }
                }
            }
            else if (manager.TouchedAnimal == gameObject) // 마우스를 뗐을때, 터치한 캐릭터가 현재 캐릭터이면
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // 터치한 캐릭터와 마우스를 뗀 위치의 거리를 구한다.
                float xdist = Mathf.Abs(pos.x - transform.position.x);
                float ydist = Mathf.Abs(pos.y - transform.position.y);

                if (xdist > ydist) // 가로 길이가 세로 길이보다 길면, 가로로 이동한 경우
                {
                    if (xdist >= 0.4f) // 최소한 터치를 0.4f 이상 움직인 경우에만 이동하게 한다.
                    {
                        int x = Index; // 현재 캐릭터의 열 번호
                        int y = manager.Block[x].IndexOf(gameObject); // 현재 캐릭터의 행 번호

                        // 터치를 뗸 X좌표가 캐릭터보다 좌측에 있고,
                        // 열 번호가 1 이상인 경우만 체크한다.
                        // 열 번호가 0인 경우는 좌측으로 이동할 수 없기 때문이다.
                        if (pos.x < transform.position.x && x >= 1) // 좌로 이동
                        {
                            manager.isMoving = true; // 모든 캐릭터의 중력을 금지시켜 못움직이게 한다,

                            // 상대방의 Animal 컴포넌트를 얻어온다.
                            target = ((GameObject)manager.Block[x - 1][y]).GetComponent<Animal>();

                            // 현재 캐릭터를 왼쪽으로 이동한다.
                            MoveX(target.transform.localPosition.x, target.ClipName);

                            // 현재 캐릭터의 왼쪽에 있는 캐릭터를 오른쪽으로 이동한다,
                            target.MoveX(transform.localPosition.x, ClipName);
                        }
                        // 터치를 뗸 X좌표가 캐릭터보다 우측에 있고,
                        // 열 번호가 4 이하인 경우만 체크한다.
                        // 열 번호가 5인(마지막 열) 경우는 우측으로 이동할 수 없기 때문이다.
                        else if (pos.x > transform.position.x && x <= 4) // 우로 이동
                        {
                            manager.isMoving = true; // 모든 캐릭터의 중력을 금지시켜 못움직이게 한다,

                            // 상대방의 Animal 컴포넌트를 얻어온다.
                            target = ((GameObject)manager.Block[x + 1][y]).GetComponent<Animal>();

                            // 현재 캐릭터를 오른쪽으로 이동한다.
                            MoveX(target.transform.localPosition.x, target.ClipName);

                            // 현재 캐릭터의 오른쪽에 있는 캐릭터를 왼쪽으로 이동한다,
                            target.MoveX(transform.localPosition.x, ClipName);
                        }
                    }
                }
                else // 세로 길이가 가로 길이보다 길면, 세로로 이동한 경우
                {
                    if (ydist >= 0.4f) // 최소한 터치를 0.4f 이상 움직인 경우에만 이동하게 한다.
                    {
                        int x = Index; // 현재 캐릭터의 열 번호
                        int y = manager.Block[x].IndexOf(gameObject); // 현재 캐릭터의 행 번호

                        // 터치를 뗸 Y좌표가 캐릭터보다 위쪽에 있고,
                        // 행 번호가 8 이하인 경우만 체크한다.
                        // 행 번호가 9인(최상단 행) 경우는 위쪽으로 이동할 수 없기 때문이다.
                        if (pos.y > transform.position.y && y <= 8) // 위로 이동
                        {
                            manager.isMoving = true; // 모든 캐릭터의 중력을 금지시켜 못움직이게 한다,

                            // 상대방의 Animal 컴포넌트를 얻어온다.
                            target = ((GameObject)manager.Block[x][y + 1]).GetComponent<Animal>();

                            // 현재 캐릭터를 위쪽으로 이동한다.
                            MoveY(target.transform.localPosition.y, target.ClipName);

                            // 현재 캐릭터의 위에 있는 캐릭터를 아래쪽으로 이동한다,
                            target.MoveY(transform.localPosition.y, ClipName);
                        }
                        // 터치를 뗸 Y좌표가 캐릭터보다 아래쪽에 있고,
                        // 행 번호가 1 이상인 경우만 체크한다.
                        // 행 번호가 0인 경우는 아래쪽으로 이동할 수 없기 때문이다.
                        else if (pos.y < transform.position.y && y >= 1) // 아래로 이동
                        {
                            manager.isMoving = true; // 모든 캐릭터의 중력을 금지시켜 못움직이게 한다,

                            // 상대방의 Animal 컴포넌트를 얻어온다.
                            target = ((GameObject)manager.Block[x][y - 1]).GetComponent<Animal>();

                            // 현재 캐릭터를 아래쪽으로 이동한다.
                            MoveY(target.transform.localPosition.y, target.ClipName);

                            // 현재 캐릭터의 아래쪽 있는 캐릭터를 위쪽으로 이동한다,
                            target.MoveY(transform.localPosition.y, ClipName);
                        }
                    }
                }
                manager.TouchedAnimal = null;
            }
        }
        GetComponent<Rigidbody>().useGravity = !manager.isMoving;
    }

    /*
     * 캐릭터를 X좌표 기준으로 이동시킨다.
     * 이동이 끝난 후에는 반대쪽에 있는 캐릭터와 ClipName이 교체되고
     * 다시 원래 위치인 oldPos로 바뀌기 때문에 반대쪽 캐릭터와
     * 위치가 바뀐것처럼 보여지는 효과가 있다.
     */
    public void MoveX(float x, string name)
    {
        NewName = name;
        oldPos = transform.localPosition;
        LeanTween.moveLocalX(gameObject, x, 0.2f).setOnComplete(MoveComplete);
    }

    /*
     * 캐릭터를 Y좌표 기준으로 이동시킨다.
     * 이동이 끝난 후에는 반대쪽에 있는 캐릭터와 ClipName이 교체되고
     * 다시 원래 위치인 oldPos로 바뀌기 때문에 반대쪽 캐릭터와
     * 위치가 바뀐것처럼 보여지는 효과가 있다.
     */
    public void MoveY(float y, string name)
    {
        NewName = name;
        oldPos = transform.localPosition;
        LeanTween.moveLocalY(gameObject, y, 0.2f).setOnComplete(MoveComplete);
    }

    /*
     * 이동후에 매치되는 캐릭터가 없는 경우
     * 원래 위치인 oldPos로 되돌아가기 위해 사용된다.
     */
    public void Move(Vector3 pos, string name)
    {
        NewName = name;
        LeanTween.moveLocal(gameObject, pos, 0.2f).setOnComplete(MoveComplete);
    }

    /*
     * 캐릭터의 이동이 끝나면 호출되는 함수
     * manager.isMoving를 false로 바꿔서 모든 캐릭터가 다시 이동할 수 있게 해준다.
     * 캐릭터의 중력(useGravity)을 manager.isMoving과 반대로 설정하고 있기 때문에
     * 캐릭터가 이동중이면 중력이 false로 되어 다른 캐릭터들이 중력의 영향을 받지 않게 되고,
     * 이동이 끝나면 다시 중력이 true로 바뀌어서 중력의 영향을 받게 된다.
     * 이것은 캐릭터 2개가 서로의 위치로 이동하는 동안 위쪽에 있는 캐릭터들이 내려오지
     * 못하게 하기 위한 것이다.
     */
    void MoveComplete()
    {
        ClipName = NewName; // 상대방 위치의 캐릭터명으로 바꿔준다.

        if (target != null) // 플레이어가 움직인 캐릭터인 경우에만 아래 루틴을 실행한다.
        {
            target.ClipName = target.NewName; // 타겟의 캐릭터명도 바꿔준다.

            // 현재 캐릭터 혹은 교환된 상대방 캐릭터 중에 1개라도 매치된 블럭이 있는지 체크한다.
            if (manager.CheckMatch(this) || manager.CheckMatch(target))
            {
                // 새로 설정된 캐릭터명으로 애니메이션을 다시 시작한다.
                // 실제로 이미지가 이 순간부터 교체된다.
                anim.Play(ClipName);
                target.anim.Play(target.ClipName);

                // 캐릭터 종류가 이미 상대방 캐릭터로 바뀐 상태이기 때문에,
                // 좌표를 원래 위치로 되돌려, 서로 교환된 것처럼 눈속임한다.
                transform.localPosition = oldPos;
                target.transform.localPosition = target.oldPos;

                manager.isMoving = false; // 캐릭터들의 중력을 다시 적용시킨다.
            }
            else // 교환된 캐릭터 2개 중에 1개도 매치되지 않은 경우
            {
                // 2개의 캐릭터를 모두 원래 위치로 돌아가게 한다.
                target.Move(target.oldPos, ClipName);
                Move(oldPos, target.ClipName);
            }
            target = null;
        }
    }

    /**
     * PuzzleManager에서 캐릭터가 사라질때 호출된다.
     * hideDelay는 이미지가 사라지는 딜레이 시간으로 터치한 순서에 따라 딜레이 시간이 점점 길어진다.
     * 순차적으로 터치한 캐릭터들이 사라지게 되는 것처럼 보이는 효과가 있다.
     * removeDelay는 실제로 오브젝트가 삭제되는 시간으로 모든 캐릭터가 같은 시간을 갖는다.
     */
    public void DestroyAnimal(float hideDelay, float removeDelay)
    {
        if (!isDead)
        {
            Invoke("Hide", hideDelay);
            Invoke("Remove", removeDelay);
            isDead = true;
        }
    }

    void Hide()
    {
        // 캐릭터의 alpha 값을 0f으로 설정해 화면에선 보이진 않지만 collider를 통한 충돌은 유지된다.
        // 즉, 이미지는 사라진 것처럼 보이지만 위쪽의 캐릭터들이 아직은 내려오지 않는다.
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

        // 사라질때 나올 DestroyEffect 프리펩을 생성한다.
        Instantiate(Effect, transform.localPosition, Quaternion.identity);
    }

    void Remove()
    {
        manager.DeleteAnimal(gameObject); // 리스트에서 현재 캐릭터를 삭제시킨다.
        manager.RebornAnimal(gameObject); // 새로운 캐릭터를 화면 위쪽에 생성한다.
        Destroy(gameObject); // 현재 캐릭터를 삭제한다.
    }
}
