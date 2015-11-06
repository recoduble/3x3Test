using UnityEngine;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public GameObject Animal;

    // 6X10 짜리 퍼즐 게임이므로 일단 가로 길이인 6개 만큼의 리스트 배열을 만든다.
    // 이 배열에 다시 어레이 리스트를 생성시켜 2차원 리스트 배열을 만들 것이다.
    public ArrayList[] Block = new ArrayList[6];

    public GameObject TouchedAnimal = null;
    public bool isMoving = false;

	void Start()
	{
        for (int i = 0; i < 6; i++)
        {
            Block[i] = new ArrayList(); // 이러면 Block[6][10] 사이즈의 2차원 리스트가 된다.
        }

        for (int x = 0; x < 6; x++) // 가로 개수
        {
            for (int y = 0; y < 10; y++) // Y 세로 개수
            {
                // 캐릭터를 생성시켜 각 컬럼에 10개씩 추가시킨다.
                Block[x].Add(CreateRandomAnimal(x, new Vector3(-2.07f + (x * 0.82f), 5f + (y * 1.2f), 0f)));
            }
        }
	}

    void Update()
    {
        DestroyMatchedBlock();
    }

    void DestroyMatchedBlock()
    {
        // 모든 캐릭터를 체크해서, 움직이고 있거나, 터져서 사라지는 중이면 리턴시킨다.
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Animal target = ((GameObject)Block[x][y]).GetComponent<Animal>();
                if (target.GetComponent<Rigidbody>().velocity.magnitude > 0.3f || target.isDead) return;
            }
        }

        // 세로 라인의 3매치를 맨 아래쪽에서 위쪽으로 체크한다.
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 8; y++) // 동시에 3개씩을 체크하므로 세로 라인은 10이 아니라 8까지만 체크한다.
            {
                // 위쪽으로 3개의 캐릭터를 가져온다.
                Animal first = ((GameObject)Block[x][y]).GetComponent<Animal>();
                Animal second = ((GameObject)Block[x][y + 1]).GetComponent<Animal>();
                Animal third = ((GameObject)Block[x][y + 2]).GetComponent<Animal>();

                // 3개의 ClipName이 서로 같으면 3개를 모두 터지도록 처리한다.
                if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
                {
                    first.DestroyAnimal(0f, 0.2f);
                    second.DestroyAnimal(0f, 0.2f);
                    third.DestroyAnimal(0f, 0.2f);
                }
            }
        }

        // 가로 라인의 3매치를 좌측부터 우측으로 체크한다.
        for (int x = 0; x < 4; x++) // 동시에 3개씩 체크하므로 가로 가인은 6이 아니라 4개까지만 체크한다.
        {
            for (int y = 0; y < 10; y++)
            {
                // 오른쪽으로 3개의 캐릭터를 가져온다.
                Animal first = ((GameObject)Block[x][y]).GetComponent<Animal>();
                Animal second = ((GameObject)Block[x + 1][y]).GetComponent<Animal>();
                Animal third = ((GameObject)Block[x + 2][y]).GetComponent<Animal>();

                // 3개의 ClipName이 서로 같으면 3개를 모두 터지도록 처리한다.
                if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
                {
                    first.DestroyAnimal(0f, 0.5f);
                    second.DestroyAnimal(0f, 0.5f);
                    third.DestroyAnimal(0f, 0.5f);
                }
            }
        }
    }

    /*
     * animal로 전달된 컴포넌트가 3매치된 상태인지 아닌지 리턴한다.
     * 기본 로직은 DestroyMatchedBlock()과 동일하다.
     */
    public bool CheckMatch(Animal animal)
    {
        int x = animal.Index; // animal의 열 번호
        int y = Block[x].IndexOf(animal.gameObject); // animal의 행 번호

        for (int i = 0; i < 8; i++)
        {
            Animal first = ((GameObject)Block[x][i]).GetComponent<Animal>();
            Animal second = ((GameObject)Block[x][i + 1]).GetComponent<Animal>();
            Animal third = ((GameObject)Block[x][i + 2]).GetComponent<Animal>();

            if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
            {
                if (first == animal || second == animal || third == animal) return true;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            Animal first = ((GameObject)Block[i][y]).GetComponent<Animal>();
            Animal second = ((GameObject)Block[i + 1][y]).GetComponent<Animal>();
            Animal third = ((GameObject)Block[i + 2][y]).GetComponent<Animal>();

            if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
            {
                if (first == animal || second == animal || third == animal) return true;
            }
        }
        return false;
    }

    public GameObject CreateRandomAnimal(int idx, Vector3 pos) // idx는 컬럼의 인덱스 번호이고, pos는 실제 생성 위치
    {
        GameObject temp = Instantiate(Animal) as GameObject;
        temp.transform.parent = transform; // 생성된 캐릭터를 PuzzleManager의 자식으로 넣는다.

        // 애니메이션 클립 이름을 char01 ~ 05까지 5종류로 생성한다. (클립은 6까지 있으므로 Range(1, 7)로도 가능하다)
        temp.GetComponent<Animal>().ClipName = string.Format("char{0:00}", Random.Range(1, 6));
        temp.GetComponent<Animal>().Index = idx; // 컬럼 번호를 Index에 넣는다.
        temp.transform.localPosition = pos;
        temp.name = "Animal";
        return temp;
    }

    public void DeleteAnimal(GameObject animal) // 캐릭터를 Block에서 삭제한다.
    {
        int x = animal.GetComponent<Animal>().Index; // 캐릭터가 속한 컬럼 번호를 가져온다.
        Block[x].Remove(animal); // 해당 컬럼에서 전달된 캐릭터를 찾아 삭제한다.
    }

    public void RebornAnimal(GameObject animal) // 캐릭터를 Block에 다시 생성한다.
    {
        int x = animal.GetComponent<Animal>().Index; // 캐릭터가 삭제된 위치의 컬럼 번호를 가져온다.

        // 화면 위쪽 5.0f부터 캐릭터를 생성하고, 이미 생성된 캐릭터가 있으면 마지막 캐릭터 좌표에서 1.2f 위쪽에 생성한다.
        float y = Mathf.Max(5.0f, ((GameObject)Block[x][Block[x].Count - 1]).transform.position.y + 1.2f);

        // 해당 컬럼의 리스트에 새로운 캐릭터를 생성해서 넣는다.
        Block[x].Add(CreateRandomAnimal(x, new Vector3(-2.07f + (x * 0.82f), y, 0f)));
    }
}
