using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Board:MonoBehaviour
{
    private Row[] rows;

    private string[] solutions;
    private string[] validWords;
    private string word;

    private int rowIndex;
    private int columnIndex;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    [Header("UI")]
    public TextMeshProUGUI invalidWordText;
    public Button newWordButton;
    public Button tryAgainButton;
    private void Awake()
    {
        rows= GetComponentsInChildren<Row>();
    }
    private void Start()
    {
        LoadData();
        NewGame();
    }
    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();
        enabled = true;
    }
    public void TryAgain()
    {
        ClearBoard();
        enabled = true;
    }
    private void LoadData()
    {
        TextAsset textFile = Resources.Load("BesHarfli") as TextAsset;
        solutions = textFile.text.Split('\n');
        validWords = solutions;
    }
    private void SetRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();
    }
    private bool IsSupportedCharacter(char c)
    {
        return "abcçdefgðhýijklmnoöprsþtuüvyz".Contains(c);
    }
    private void Update()
    {
        if (rowIndex >= rows.Length)
            return;

        Row currentRow = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);

            invalidWordText.gameObject.SetActive(false);
        }
        else if (columnIndex >= rows[rowIndex].tiles.Length)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }
        else
        {
            string input = Input.inputString;

            if (!string.IsNullOrEmpty(input))
            {
                foreach (char c in input.ToLower())
                {
                    if (IsSupportedCharacter(c))
                    {
                        currentRow.tiles[columnIndex].SetLetter(c);
                        currentRow.tiles[columnIndex].SetState(occupiedState);
                        columnIndex++;
                        break;
                    }
                }
            }

        }
    }
    private void SubmitRow(Row row)
    {
        if (!IsValidWord(row.word))
        {
            invalidWordText.gameObject.SetActive(true);
            return;
        }

        string remaining = word;
        for (int i = 0; i < row.tiles.Length; i++) { 
            Tile tile= row.tiles[i];
            if (tile.letter == word[i])
            {
                tile.SetState(correctState);
                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!word.Contains(tile.letter)) { 
                tile.SetState(incorrectState);
            }
        }

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];
            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);
                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }
        }
        if (HasWon(row))
        {
            enabled= false;
        }
        rowIndex++;
        columnIndex = 0;

        if(rowIndex >= rows.Length)
        {
            enabled = false;
        }
            
    }
    private void ClearBoard()
    {
        for (int row = 0; row < rows.Length; row++) 
        {
            for (int col = 0; col < rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }
        rowIndex = 0;
        columnIndex = 0;
    }
    private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++) 
        { 
            if (validWords[i] == word)
            {
                return true;
            }
        }
        return false;

    }

    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++) 
        { 
            if(row.tiles[i].state != correctState)
            {
                return false;
            }
        }
        return true;
    }
    private void OnEnable()
    {
        tryAgainButton.gameObject.SetActive(false);
        newWordButton.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        tryAgainButton.gameObject.SetActive(true);
        newWordButton.gameObject.SetActive(true);
    }

}
