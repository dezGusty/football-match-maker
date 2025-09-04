[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download)
[![Angular](https://img.shields.io/badge/Angular-20-DD0031)](https://angular.io/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED)](https://www.docker.com/)

Football Match Maker is a modern web application designed to help football enthusiasts organize matches, manage teams, and coordinate players efficiently. Built with .NET 9 backend and Angular 20 frontend, it provides a robust platform for football community management.

## 🌐 Live Demo

<div align="center">

### ⚽ Experience Football Match Maker in Action
[**football.t1f1.com**](https://football.t1f1.com)

[![Demo Status](https://img.shields.io/website?url=https%3A%2F%2Ffootball.t1f1.com&label=demo%20status&style=for-the-badge)](https://football.t1f1.com)
[![Development](https://img.shields.io/badge/status-in%20development-orange?style=for-the-badge)](https://github.com/dezGusty/football-match-maker)

</div>

> **ℹ️ Development Status**: This project is actively under development. The demo environment might experience occasional downtime or instability as we implement new features and improvements. We appreciate your understanding and patience!

### 📞 Need Help?
If you encounter any issues accessing the demo:
- 📧 Contact us
- ⏰ Try again in a few minutes

> **Pro Tip**: The demo environment is refreshed daily at 00:00 UTC to ensure the best experience for all users.
> 
> **Note**: During development, the demo might be temporarily unavailable. This is normal and helps us deliver a better product!

## 🌟 Features

- **Match Organization**: Create and manage football matches with ease
- **Player Profiles**: Detailed player profiles with ratings
- **Friend System**: Connect with other players and build your network
- **Real-time Updates**: Get instant notifications about match updates
- **Responsive Design**: Works seamlessly on desktop and mobile devices

## 🚀 Getting Started

### Prerequisites

Make sure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (v16 or higher)
- [Angular CLI](https://cli.angular.io/) (`npm install -g @angular/cli`)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/install/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (for local development)
- IDE of choice ([Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/))

### Development Setup

1. **Clone the repository**
```bash
git clone https://github.com/dezGusty/football-match-maker.git
cd football-match-maker
```

2. **Set up the backend**
```bash
cd FootballAPI
dotnet restore
dotnet ef database update
dotnet run
```

3. **Set up the frontend**
```bash
cd ../FootballClient
npm install
ng serve
```

The application will be available at:
- Frontend: `http://localhost:4200`
- Backend API: `http://localhost:5145`

## 🏗️ Project Structure

The project follows a clean architecture pattern with separate frontend and backend applications:

```
football-match-maker/
├── FootballAPI/                # .NET Backend
│   ├── Controllers/          
│   ├── Models/              
│   ├── DTOs/                
│   ├── Services/            
│   ├── Repository/          
│   └── AppDbContext/       
│
├── FootballClient/           # Angular Frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/  
│   │   │   ├── services/    
│   │   │   ├── models/     
│   │   │   └── guards/     
│   │   └── assets/        
│   │
│   └── nginx.conf           # Nginx configuration
│
├── docker-compose.yml        # Docker configuration
└── README.md                # Documentation

```

## 🎯 Key Features Implementation

### Authentication
- JWT-based authentication
- Password reset functionality
- Role-based authorization

### Match Management
- Create/Edit/Delete matches
- Team assignment
- Player registration
- Match status tracking

### Team Features
- Team creation and management
- Player invitations
- Team statistics
- Historical match data

## 🐳 Production Deployment

### Docker Deployment

1. **Configure Environment Variables**

Create a `prod.env` file in the root directory:
```env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=db;Database=FootballDB;User=sa;Password=YourStrongPassword!
JWT_SECRET=your_jwt_secret_key
CORS_ORIGINS=https://yourdomain.com
```

2. **Build and Run Docker Containers**
```bash
docker compose up --build -d
```

This will start:
- SQL Server database
- .NET API backend
- Angular frontend with Nginx
- Prometheus monitoring (optional)

### 🌍 Nginx Reverse Proxy Setup

For production deployment, configure Nginx as a reverse proxy:

1. **Create Nginx Configuration**

```nginx
# /etc/nginx/sites-available/football-match-maker.conf
server {
    listen 80;
    listen [::]:80;
    server_name yourdomain.com;
    
    # Redirect HTTP to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name yourdomain.com;

    # SSL Configuration
    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    ssl_session_timeout 1d;
    ssl_session_cache shared:SSL:50m;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;

    # Frontend
    location / {
        proxy_pass http://localhost:4200;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # Backend API
    location /api/ {
        proxy_pass http://localhost:5145;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "Upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;

        # CORS headers
        add_header 'Access-Control-Allow-Origin' 'https://yourdomain.com' always;
        add_header 'Access-Control-Allow-Methods' 'GET, POST, PUT, DELETE, OPTIONS' always;
        add_header 'Access-Control-Allow-Headers' 'DNT,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Range,Authorization' always;
    }

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
    add_header Content-Security-Policy "default-src 'self' https: data: 'unsafe-inline' 'unsafe-eval';" always;
}
```

2. **Enable Configuration**
```bash
sudo ln -s /etc/nginx/sites-available/football-match-maker.conf /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## 🔒 SSL Configuration

Install and configure SSL certificates using Let's Encrypt:

```bash
# Install Certbot
sudo apt install certbot python3-certbot-nginx -y

# Generate SSL certificates
sudo certbot --nginx -d yourdomain.com

# Auto-renewal
sudo certbot renew --dry-run
```

## � Monitoring

The application includes Prometheus metrics for monitoring:

- **API Metrics**: Available at `https://yourdomain.com/metrics`
- **Key Metrics**:
  - HTTP request duration
  - Active users
  - Match creation rate
  - Database connection pool status
  - Memory usage

## 🔍 Troubleshooting Guide

### Common Issues

1. **API Connection Failed**
   - Check if the API container is running: `docker ps`
   - Verify API logs: `docker logs football-api`
   - Ensure correct environment variables in `prod.env`

2. **Database Connection Issues**
   - Verify SQL Server is running: `docker ps | grep db`
   - Check connection string in environment variables
   - Review database logs: `docker logs football-db`

3. **Frontend Loading Problems**
   - Clear browser cache
   - Check browser console for errors
   - Verify Nginx configuration
   - Review frontend logs: `docker logs football-client`

### Security Checks

- Regular security updates: `docker-compose pull`
- SSL certificate status: `certbot certificates`
- Nginx configuration test: `nginx -t`



## 🙏 Acknowledgments

- [ASP.NET Core Team](https://github.com/dotnet/aspnetcore)
- [Angular Team](https://angular.io/docs/ts/latest/)
