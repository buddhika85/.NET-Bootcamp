import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd())
  const backendUrl = env.VITE_BACKEND_API_URL

  return {
    plugins: [react()],
    server: {
      proxy: {
        '/games': backendUrl,
        '/genres': backendUrl,
      },
    },
  }
})
