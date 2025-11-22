import { test, expect } from '@playwright/test'

test('user can create a new task', async ({ page }) => {
  await page.goto('/')

  await page.getByRole('button', { name: /add new task/i }).click()
  await page.getByLabel(/title/i).fill('E2E create task')
  await page.getByRole('button', { name: /add task/i }).click()

  await expect(page.getByText('E2E create task')).toBeVisible()
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

  await expect(row.getByText(/completed|pending/i)).toBeVisible()
})


