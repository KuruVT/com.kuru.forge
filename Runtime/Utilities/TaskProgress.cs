using UnityEditor;

using UnityEngine;

namespace Forge.Utilities
{
    public static class TaskProgress
    {
        private static string ProgressTitle = "Progress";
        private static string TaskDescription = string.Empty;
        private static float CurrentProgress = 0f;
        private static bool IsProgressActive = false;

        /// <summary>
        /// Initializes the progress bar with a specific title.
        /// </summary>
        /// <param name="progressTitle">The title displayed on the progress bar.</param>
        public static void Initialize(string progressTitle = "Progress")
        {
            ProgressTitle = progressTitle;
            CurrentProgress = 0f;
            IsProgressActive = true;
            ExecuteSafely(() => EditorUtility.DisplayProgressBar(ProgressTitle, "Starting...", CurrentProgress));
        }

        /// <summary>
        /// Updates the progress bar with a custom task description and progress value.
        /// </summary>
        /// <param name="taskDescription">The description of the current task.</param>
        /// <param name="progressValue">Progress value between 0.0 and 1.0.</param>
        public static void Update(string taskDescription, float progressValue)
        {
            if (!IsProgressActive) return;

            TaskDescription = taskDescription;
            CurrentProgress = Mathf.Clamp01(progressValue);
            ExecuteSafely(() => EditorUtility.DisplayProgressBar(ProgressTitle, TaskDescription, CurrentProgress));
        }

        /// <summary>
        /// Updates the progress bar for batch operations based on item index and total count.
        /// </summary>
        /// <param name="totalItems">Total number of items being processed.</param>
        /// <param name="currentItemName">Name of the current item being processed.</param>
        /// <param name="currentIndex">Index of the current item in the batch.</param>
        public static void UpdateBatch(int totalItems, string currentItemName, long currentIndex)
        {
            if (!IsProgressActive) return;

            float progressValue = Mathf.Clamp01((float)currentIndex / totalItems);
            string taskDescription = $"Processing {currentItemName} ({currentIndex + 1}/{totalItems})...";
            Update(taskDescription, progressValue);
        }

        /// <summary>
        /// Completes and clears the progress bar from the screen.
        /// </summary>
        public static void Complete()
        {
            if (!IsProgressActive) return;

            ExecuteSafely(EditorUtility.ClearProgressBar);
            IsProgressActive = false;
            CurrentProgress = 0f;
        }

        /// <summary>
        /// Ensures that the provided action runs on the Unity main thread safely.
        /// Automatically cancels the progress bar on error.
        /// </summary>
        /// <param name="action">The action to run on the main thread.</param>
        private static void ExecuteSafely(System.Action action)
        {
            try
            {
                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (System.Exception ex)
                    {
                        HandleError(ex);
                    }
                };
            }
            catch (System.Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Handles any errors by clearing the progress bar and logging the issue.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        private static void HandleError(System.Exception exception)
        {
            if (IsProgressActive)
            {
                EditorUtility.ClearProgressBar();
                IsProgressActive = false;
                Debug.LogError($"[TaskProgress] Progress bar canceled due to an error: {exception.Message}");
            }
        }
    }
}
