import React, { useEffect, useState } from 'react'
import ReactDOM from 'react-dom/client'
import './style.css'

type TaskStatus = 'Todo' | 'InProgress' | 'Done'
type TaskPriority = 'Low' | 'Medium' | 'High'

type Task = {
  id: string
  title: string
  description?: string | null
  status: TaskStatus
  priority: TaskPriority
  dueDate?: string | null
  createdAt: string
  updatedAt: string
}

type LoadState = 'idle' | 'loading' | 'loaded' | 'error'

const App: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([])
  const [loadState, setLoadState] = useState<LoadState>('idle')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchTasks = async () => {
      try {
        setLoadState('loading')
        setError(null)

        const response = await fetch('/api/tasks')
        if (!response.ok) {
          throw new Error(`Request failed with status ${response.status}`)
        }

        const data = (await response.json()) as Task[]
        setTasks(data)
        setLoadState('loaded')
      } catch (err) {
        console.error(err)
        setError('Unable to load tasks from the server.')
        setLoadState('error')
      }
    }

    fetchTasks()
  }, [])

  return React.createElement(
    'main',
    { className: 'app' },
    React.createElement(
      'header',
      { className: 'app-header' },
      React.createElement('h1', null, 'Atomic Tasks'),
      React.createElement(
        'p',
        { className: 'app-subtitle' },
        'A simple task management app built with .NET 9, React, and SQLite.'
      )
    ),
    React.createElement(
      'section',
      { className: 'app-section' },
      loadState === 'loading' &&
        React.createElement('p', null, 'Loading tasks…'),
      loadState === 'error' && error &&
        React.createElement('p', null, error),
      loadState === 'loaded' && tasks.length === 0 &&
        React.createElement('p', null, 'No tasks found yet.'),
      tasks.length > 0 &&
        React.createElement(
          'table',
          { className: 'task-table' },
          React.createElement(
            'thead',
            null,
            React.createElement(
              'tr',
              null,
              React.createElement('th', null, 'Title'),
              React.createElement('th', null, 'Status'),
              React.createElement('th', null, 'Priority'),
              React.createElement('th', null, 'Due'),
              React.createElement('th', null, 'Created')
            )
          ),
          React.createElement(
            'tbody',
            null,
            tasks.map((task) =>
              React.createElement(
                'tr',
                { key: task.id },
                React.createElement('td', null, task.title),
                React.createElement(
                  'td',
                  null,
                  React.createElement(
                    'span',
                    { className: `pill pill-${task.status.toLowerCase()}` },
                    task.status
                  )
                ),
                React.createElement('td', null, task.priority),
                React.createElement(
                  'td',
                  null,
                  task.dueDate ? new Date(task.dueDate).toLocaleDateString() : '—'
                ),
                React.createElement(
                  'td',
                  null,
                  new Date(task.createdAt).toLocaleString()
                )
              )
            )
          )
        ),
      loadState === 'idle' &&
        React.createElement('p', null, 'Preparing to load tasks…'),
      React.createElement(
        'p',
        { style: { marginTop: '1.5rem', fontSize: '0.85rem', color: '#9ca3af' } },
        'This view currently shows tasks fetched from the backend. Next steps will add creation, editing, deletion, and status changes from the UI.'
      )
    )
  )
}

ReactDOM.createRoot(document.getElementById('app') as HTMLElement).render(
  React.createElement(React.StrictMode, null, React.createElement(App))
)
