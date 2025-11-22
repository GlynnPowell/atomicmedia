import React, { useEffect, useState } from 'react'
import ReactDOM from 'react-dom/client'
import './style.css'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api'

type Task = {
  id: number
  title: string
  description?: string | null
  isCompleted: boolean
  dueDate?: string | null
  createdAt: string
  updatedAt: string
}

type LoadState = 'idle' | 'loading' | 'loaded' | 'error'

type Route =
  | { kind: 'list' }
  | { kind: 'new' }
  | { kind: 'edit'; id: number }

const parseRoute = (pathname: string): Route => {
  if (pathname === '/tasks/new') {
    return { kind: 'new' }
  }

  const editMatch = pathname.match(/^\/tasks\/(\d+)$/)
  if (editMatch) {
    return { kind: 'edit', id: Number(editMatch[1]) }
  }

  // Default to list view
  return { kind: 'list' }
}

const App: React.FC = () => {
  const [route, setRoute] = useState<Route>(() => parseRoute(window.location.pathname))
  const [tasks, setTasks] = useState<Task[]>([])
  const [loadState, setLoadState] = useState<LoadState>('idle')
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const [editingId, setEditingId] = useState<number | null>(null)
  const [titleInput, setTitleInput] = useState('')
  const [descriptionInput, setDescriptionInput] = useState('')
  const [dueDateInput, setDueDateInput] = useState('')
  const [formError, setFormError] = useState<string | null>(null)

  useEffect(() => {
    const handlePopState = () => {
      setRoute(parseRoute(window.location.pathname))
    }

    window.addEventListener('popstate', handlePopState)
    return () => window.removeEventListener('popstate', handlePopState)
  }, [])

  useEffect(() => {
    const fetchTasks = async () => {
      try {
        setLoadState('loading')
        setError(null)

        const response = await fetch(`${API_BASE_URL}/tasks`)
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

  const resetForm = () => {
    setEditingId(null)
    setTitleInput('')
    setDescriptionInput('')
    setDueDateInput('')
    setFormError(null)
  }

  const navigate = (next: Route) => {
    let path = '/tasks'
    if (next.kind === 'new') {
      path = '/tasks/new'
    } else if (next.kind === 'edit') {
      path = `/tasks/${next.id}`
    }

    window.history.pushState(null, '', path)
    setRoute(next)
  }

  // Keep form state in sync with the current route
  useEffect(() => {
    if (route.kind === 'new') {
      resetForm()
    } else if (route.kind === 'edit') {
      const task = tasks.find(t => t.id === route.id)
      if (task) {
        setEditingId(task.id)
        setTitleInput(task.title)
        setDescriptionInput(task.description ?? '')
        setDueDateInput(task.dueDate ? task.dueDate.substring(0, 10) : '')
        setFormError(null)
      }
    } else {
      // list
      resetForm()
    }
  }, [route, tasks])

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (isSubmitting) return

    const trimmedTitle = titleInput.trim()
    if (!trimmedTitle) {
      setFormError('Title is required.')
      return
    }
    if (trimmedTitle.length > 100) {
      setFormError('Title must be at most 100 characters.')
      return
    }

    setIsSubmitting(true)
    setFormError(null)

    try {
      const isEdit = Boolean(editingId)
      const url = isEdit ? `${API_BASE_URL}/tasks/${editingId}` : `${API_BASE_URL}/tasks`
      const method = isEdit ? 'PUT' : 'POST'

      const body: any = {
        title: trimmedTitle,
        description: descriptionInput || null,
        dueDate: dueDateInput ? new Date(dueDateInput).toISOString() : null
      }

      if (isEdit) {
        const existing = tasks.find(t => t.id === editingId)
        body.isCompleted = existing?.isCompleted ?? false
      }

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
      })

      if (!response.ok) {
        const problemText = await response.text()
        throw new Error(`Save failed (${response.status}): ${problemText || 'Unknown error'}`)
      }

      const saved = (await response.json()) as Task

      setTasks(prev => {
        const others = prev.filter(t => t.id !== saved.id)
        return [saved, ...others]
      })

      resetForm()
      if (loadState === 'idle') {
        setLoadState('loaded')
      }
    } catch (err) {
      console.error(err)
      setFormError('Unable to save the task. Please try again.')
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this task?')) {
      return
    }

    try {
      const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
        method: 'DELETE'
      })
      if (!response.ok && response.status !== 404) {
        throw new Error(`Delete failed (${response.status})`)
      }

      setTasks(prev => prev.filter(t => t.id !== id))
      if (editingId === id) {
        resetForm()
      }
    } catch (err) {
      console.error(err)
      alert('Unable to delete the task.')
    }
  }

  const handleCompletionToggle = async (id: number, isCompleted: boolean) => {
    try {
      const existing = tasks.find(t => t.id === id)
      if (!existing) return

      const response = await fetch(`${API_BASE_URL}/tasks/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          title: existing.title,
          description: existing.description,
          isCompleted,
          dueDate: existing.dueDate
        })
      })

      if (!response.ok) {
        throw new Error(`Status update failed (${response.status})`)
      }

      const updated = (await response.json()) as Task
      setTasks(prev => prev.map(t => (t.id === id ? updated : t)))
    } catch (err) {
      console.error(err)
      alert('Unable to update task status.')
    }
  }

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
      // Simple navigation
      React.createElement(
        'div',
        { className: 'task-nav' },
        route.kind !== 'list' &&
          React.createElement(
            'button',
            {
              type: 'button',
              className: 'link',
              onClick: () => navigate({ kind: 'list' })
            },
            '← Back to tasks'
          ),
        route.kind === 'list' &&
          React.createElement(
            'button',
            {
              type: 'button',
              className: 'link',
              onClick: () => navigate({ kind: 'new' })
            },
            'Add new task'
          )
      ),
      // Add / Edit task view
      (route.kind === 'new' || route.kind === 'edit') &&
        React.createElement(
          'form',
          {
            className: 'task-form',
            onSubmit: handleSubmit
          },
          React.createElement(
            'div',
            { className: 'task-form-row' },
            React.createElement(
              'label',
              null,
              'Title',
              React.createElement('input', {
                type: 'text',
                value: titleInput,
                onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
                  setTitleInput(e.target.value),
                disabled: isSubmitting
              })
            ),
            React.createElement(
              'label',
              null,
              'Due date',
              React.createElement('input', {
                type: 'date',
                value: dueDateInput,
                onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
                  setDueDateInput(e.target.value),
                disabled: isSubmitting
              })
            )
          ),
          React.createElement(
            'div',
            { className: 'task-form-row' },
            React.createElement(
              'label',
              { className: 'task-form-description' },
              'Description',
              React.createElement('textarea', {
                value: descriptionInput,
                rows: 2,
                onChange: (e: React.ChangeEvent<HTMLTextAreaElement>) =>
                  setDescriptionInput(e.target.value),
                disabled: isSubmitting
              })
            )
          ),
          formError &&
            React.createElement(
              'p',
              { className: 'task-form-error' },
              formError
            ),
          React.createElement(
            'div',
            { className: 'task-form-actions' },
            React.createElement(
              'button',
              { type: 'submit', disabled: isSubmitting },
              route.kind === 'edit' ? 'Update task' : 'Add task'
            ),
            React.createElement(
              'button',
              {
                type: 'button',
                className: 'secondary',
                onClick: () => navigate({ kind: 'list' }),
                disabled: isSubmitting
              },
              'Cancel'
            )
          )
        ),
      // Task list view
      route.kind === 'list' &&
        React.createElement(
          React.Fragment,
          null,
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
                  React.createElement('th', null, 'Due'),
                  React.createElement('th', null, 'Created'),
                  React.createElement('th', null, 'Actions')
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
                      task.isCompleted
                        ? React.createElement('span', { className: 'pill pill-done' }, 'Completed')
                        : React.createElement('span', { className: 'pill pill-todo' }, 'Pending')
                    ),
                    React.createElement(
                      'td',
                      null,
                      task.dueDate ? new Date(task.dueDate).toLocaleDateString() : '—'
                    ),
                    React.createElement(
                      'td',
                      null,
                      new Date(task.createdAt).toLocaleString()
                    ),
                    React.createElement(
                      'td',
                      { className: 'task-actions' },
                      React.createElement(
                        'button',
                        {
                          type: 'button',
                          className: 'link',
                          onClick: () => handleCompletionToggle(task.id, !task.isCompleted)
                        },
                        task.isCompleted ? 'Mark as pending' : 'Mark as completed'
                      ),
                      React.createElement(
                        'button',
                        {
                          type: 'button',
                          className: 'link',
                          onClick: () => navigate({ kind: 'edit', id: task.id })
                        },
                        'Edit'
                      ),
                      React.createElement(
                        'button',
                        {
                          type: 'button',
                          className: 'link destructive',
                          onClick: () => handleDelete(task.id)
                        },
                        'Delete'
                      )
                    )
                  )
                )
              )
            )
        )
    )
  )
}

ReactDOM.createRoot(document.getElementById('app') as HTMLElement).render(
  React.createElement(React.StrictMode, null, React.createElement(App))
)
