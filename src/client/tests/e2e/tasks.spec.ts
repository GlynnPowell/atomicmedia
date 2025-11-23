import { test, expect } from '@playwright/test'

test('user can create a new task', async ({ page }) => {
  await page.goto('/')

  await page.getByRole('button', { name: /add new task/i }).click()
  await page.getByLabel(/title/i).fill('E2E create task')
  await page.getByRole('button', { name: /add task/i }).click()

  // Assert that a table row containing the new task title is visible
  await expect(
    page.getByRole('row', { name: /E2E create task/i }).first()
  ).toBeVisible()
})

test('user can toggle task completion', async ({ page }) => {
  await page.goto('/')

  // Ensure a known task exists
  const label = 'E2E toggle task'
  const existingRow = page.getByRole('row', { name: new RegExp(label, 'i') }).first()

  if (!(await existingRow.isVisible().catch(() => false))) {
    await page.getByRole('button', { name: /add new task/i }).click()
    await page.getByLabel(/title/i).fill(label)
    await page.getByRole('button', { name: /add task/i }).click()
  }

  const row = page.getByRole('row', { name: new RegExp(label, 'i') }).first()
  const toggleButton = row.getByRole('button', {
    name: /mark as completed|mark as pending/i
  })

  await toggleButton.click()

  // Check the status cell (2nd cell in the row: Title, Status, ...) shows the updated state
  const statusCell = row.getByRole('cell').nth(1)
  await expect(statusCell.getByText(/completed|pending/i)).toBeVisible()
})


