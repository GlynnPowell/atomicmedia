import React from 'react'
import ReactDOM from 'react-dom/client'
import './style.css'

const App: React.FC = () => {
  return (
    <main className="app">
      <header className="app-header">
        <h1>Atomic Tasks</h1>
        <p className="app-subtitle">
          A simple task management app built with .NET 9, React, and SQLite.
        </p>
      </header>
      <section className="app-section">
        <p>
          Frontend scaffold is ready. Next steps: implement task list, creation, editing, and status
          updates against the `/api/tasks` backend.
        </p>
      </section>
    </main>
  )
}

ReactDOM.createRoot(document.getElementById('app') as HTMLElement).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)
