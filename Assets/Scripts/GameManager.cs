using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리자

public class GameManager : MonoBehaviour
{
    // [추가] 일시정지 UI 패널을 연결할 변수
    public GameObject pauseMenuPanel; 
    public GameObject missionClearPanel;
    
    // [추가] 별 UI 이미지들 (Inspector에서 연결)
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    // [추가] 현재 일시정지 상태인지 확인하는 변수
    private bool isPaused = false;

    // [추가] Update 함수. 매 프레임 키 입력을 감지합니다.
    void Update()
    {
        // 'P' 키를 눌렀는지 확인
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                // 이미 멈춘 상태면, 게임 재개
                ResumeGame();
            }
            else
            {
                // 실행 중인 상태면, 게임 일시정지
                PauseGame();
            }
        }
    }

    // [추가] 게임을 일시정지 시키는 함수
    void PauseGame()
    {
        isPaused = true;
        
        // (중요) 게임 내의 시간을 0배속으로 만들어 멈춥니다.
        Time.timeScale = 0f; 
        
        // (중요) 숨겨뒀던 팝업창(패널)을 켭니다.
        pauseMenuPanel.SetActive(true);
    }

    // [추가] 게임을 다시 재개하는 함수 (버튼이 쓸 수 있게 public으로!)
    public void ResumeGame()
    {
        isPaused = false;

        // (중요) 게임 시간을 다시 1배속(정상)으로 돌립니다.
        Time.timeScale = 1f; 
        
        // (중요) 팝업창(패널)을 다시 끕니다.
        pauseMenuPanel.SetActive(false);
    }

    // [추가] 맵을 재시작하는 함수 (버튼이 쓸 수 있게 public으로!)
    public void RestartMap()
    {
        // (필수!) 멈췄던 시간을 다시 1배속으로 돌려놔야 합니다.
        // 그렇지 않으면 씬이 리로드돼도 계속 멈춰있습니다.
        Time.timeScale = 1f;

        // 현재 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 기존의 PlayerDied 함수는 그대로 둡니다.
    public void PlayerDied()
    {
        Debug.Log("플레이어가 사망했습니다. 씬을 다시 시작합니다.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // [수정] 미션 클리어 함수 - 남은 shot 수를 받아서 별 표시
    public void LevelComplete(int remainingShots)
    {
        Debug.Log("미션 클리어! 남은 shots: " + remainingShots);

        // 1. 게임 시간을 멈춥니다. (Pause와 동일)
        Time.timeScale = 0f;

        // 2. 숨겨뒀던 "미션 클리어" 패널을 켭니다.
        missionClearPanel.SetActive(true);
        
        // 3. 남은 shot 수에 따라 별 표시
        int stars = CalculateStars(remainingShots);
        ShowStars(stars);
    }
    
    // 남은 shot 수에 따라 별 개수 계산
    private int CalculateStars(int remainingShots)
    {
        if (remainingShots >= 5)
            return 3; // 별 3개 (완벽!)
        else if (remainingShots >= 3)
            return 2; // 별 2개 (좋음)
        else
            return 1; // 별 1개 (클리어)
    }
    
    // 별 UI 표시
    private void ShowStars(int starCount)
    {
        // 모든 별 비활성화
        if (star1 != null) star1.SetActive(false);
        if (star2 != null) star2.SetActive(false);
        if (star3 != null) star3.SetActive(false);
        
        // starCount에 따라 별 활성화
        if (starCount >= 3 && star3 != null) star3.SetActive(true);
        else if (starCount >= 2 && star2 != null) star2.SetActive(true);
        else if (starCount >= 1 && star1 != null) star1.SetActive(true);

        Debug.Log("별 " + starCount + "개 획득!");
    }

    // [수정] "다음 레벨" 버튼이 호출할 함수
    public void LoadNextLevel()
    {
        // 1. (필수!) 멈췄던 시간을 다시 1배속으로 돌려놓습니다.
        Time.timeScale = 1f;

        // 2. 현재 활성화된 씬의 '빌드 인덱스'를 가져옵니다.
        // (예: Level 1이 1번이라면, currentSceneIndex는 1이 됩니다.)
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 3. 다음 씬의 인덱스를 계산합니다. (현재 인덱스 + 1)
        int nextSceneIndex = currentSceneIndex + 1;

        // 4. (선택 사항) 만약 다음 씬이 마지막 씬이라면?
        // SceneManager.sceneCountInBuildSettings는 빌드 세팅에 등록된 "총 씬의 개수"입니다.
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            // 다음 씬이 없으므로, 0번 인덱스(보통 메인 메뉴)로 돌아가게 합니다.
            // (이 if 문이 필요 없다면 4, 5번 줄은 지우고 8번 줄만 남기세요)
            SceneManager.LoadScene(0); 
        }
        else
        {
            // 5. 다음 씬(계산된 인덱스 번호)을 불러옵니다.
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}