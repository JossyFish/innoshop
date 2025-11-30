import type { NextConfig } from "next"

const nextConfig: NextConfig = {
  async rewrites() {
    return [
      {
        source: '/api/users/:path*',
        destination: 'http://userservice:8080/api/:path*'
      },
      {
        source: '/api/products/:path*', 
        destination: 'http://productservice:8080/api/:path*'
      }
    ]
  }
}

export default nextConfig