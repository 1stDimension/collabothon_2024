import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  output: "standalone",
  rewrites: async () => [
    {
      source: '/api/:path*',
      destination: 'http://localhost:8080/api/:path*'
    },
    {
      source: '/vertex/:path*',
      destination: 'http://localhost:8080/vertex/:path*'
    }
  ]
};

export default nextConfig;
