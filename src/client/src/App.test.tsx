import React from 'react'
import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { App } from './main'

const mockFetch = (tasks: any[]) => {
  vi.spyOn(globalThis, 'fetch').mockImplementation((input: RequestInfo | URL) => {
    const url = input.toString()

    if (url.includes('/tasks/filter-values')) {
      return Promise.resolve({
        ok: true,
        json: async () => ({
          createdBy: [],
          assignedTo: []
        })
      } as any)
    }

    // Default: task list
    return Promise.resolve({
      ok: true,
      json: async () => tasks
    } as any)
  })
}

describe('App', () => {
  it('renders the application header', async () => {
    mockFetch([])

    render(React.createElement(App))

    expect(await screen.findByRole('heading', { name: /atomic tasks/i })).toBeInTheDocument()
  })

  it('validates that title is required when adding a task', async () => {
    mockFetch([])
    const user = userEvent.setup()

    render(React.createElement(App))

    const addButton = await screen.findByRole('button', { name: /add new task/i })
    await user.click(addButton)

    const submitButton = await screen.findByRole('button', { name: /add task/i })
    await user.click(submitButton)

    expect(await screen.findByText(/title is required/i)).toBeInTheDocument()
  })

  it('shows tasks returned from the API', async () => {
    const now = new Date().toISOString()

    mockFetch([
      {
        id: 1,
        title: 'Write assessment',
        description: null,
        isCompleted: false,
        dueDate: null,
        createdAt: now,
        updatedAt: now
      }
    ])

    render(React.createElement(App))

    expect(await screen.findByText('Write assessment')).toBeInTheDocument()
    expect(await screen.findByText(/pending/i)).toBeInTheDocument()
  })
})


