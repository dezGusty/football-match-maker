import { Component, HostListener, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { UserRole } from '../../models/user-role.enum';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class Login implements OnInit {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  // Interactive ball properties
  ballPosition = { x: 30, y: window.innerHeight - 130 }; // Start from bottom-left
  isDragging = false;
  isBouncing = false;
  isThrowable = false;
  private dragStart = { x: 0, y: 0 };
  private mouseStart = { x: 0, y: 0 };
  private velocity = { x: 0, y: 0 };
  private lastMousePos = { x: 0, y: 0 };
  private lastTime = 0;

  constructor(
    private router: Router,
    private authService: AuthService,
    private notificationService: NotificationService
  ) {
    if (this.authService.isLoggedIn()) {
      this.redirectBasedOnRole();
    }
  }
  ngOnInit(): void {
    // Set initial ball position from bottom-left
    this.ballPosition = { 
      x: 30, 
      y: window.innerHeight - 130 
    };
    
    let index = 0;
    const slides =
      document.querySelectorAll<HTMLImageElement>('.slideshow .slide');

    setInterval(() => {
      slides.forEach((slide, i) => {
        slide.classList.toggle('active', i === index);
      });
      index = (index + 1) % slides.length;
    }, 4000);

    // Initialize ball physics
    this.setupBallPhysics();
  }

  private setupBallPhysics(): void {
    // Reset ball position on window resize
    window.addEventListener('resize', () => {
      this.constrainBallPosition();
    });
  }

  // Ball interaction methods
  startDrag(event: MouseEvent | TouchEvent): void {
    event.preventDefault();
    this.isDragging = true;
    this.isBouncing = false;
    
    const clientX = event instanceof MouseEvent ? event.clientX : event.touches[0].clientX;
    const clientY = event instanceof MouseEvent ? event.clientY : event.touches[0].clientY;
    
    this.dragStart = { x: this.ballPosition.x, y: this.ballPosition.y };
    this.mouseStart = { x: clientX, y: clientY };
    this.lastMousePos = { x: clientX, y: clientY };
    this.lastTime = Date.now();
    
    document.body.style.userSelect = 'none';
  }

  @HostListener('document:mousemove', ['$event'])
  @HostListener('document:touchmove', ['$event'])
  onMouseMove(event: MouseEvent | TouchEvent): void {
    if (!this.isDragging) return;
    
    event.preventDefault();
    const clientX = event instanceof MouseEvent ? event.clientX : event.touches[0].clientX;
    const clientY = event instanceof MouseEvent ? event.clientY : event.touches[0].clientY;
    
    // Calculate velocity for throwing
    const currentTime = Date.now();
    const timeDelta = currentTime - this.lastTime;
    
    if (timeDelta > 0) {
      this.velocity.x = (clientX - this.lastMousePos.x) / timeDelta * 10;
      this.velocity.y = (clientY - this.lastMousePos.y) / timeDelta * 10;
    }
    
    this.lastMousePos = { x: clientX, y: clientY };
    this.lastTime = currentTime;
    
    // Update ball position - allow movement across entire screen
    this.ballPosition.x = Math.max(0, Math.min(window.innerWidth - 100, clientX - 50));
    this.ballPosition.y = Math.max(0, Math.min(window.innerHeight - 100, clientY - 50));
  }

  @HostListener('document:mouseup', ['$event'])
  @HostListener('document:touchend', ['$event'])
  onMouseUp(event: MouseEvent | TouchEvent): void {
    if (!this.isDragging) return;
    
    this.isDragging = false;
    document.body.style.userSelect = '';
    
    // Apply throwing physics if there's enough velocity
    const speed = Math.sqrt(this.velocity.x * this.velocity.x + this.velocity.y * this.velocity.y);
    
    if (speed > 2) {
      this.throwBall();
    } else {
      this.bounceBall();
    }
  }

  private throwBall(): void {
    const ball = document.querySelector('.football-ball') as HTMLElement;
    if (!ball) return;
    
    ball.classList.add('throwing');
    
    // Enhanced physics with gravity and momentum conservation
    let newX = this.ballPosition.x + this.velocity.x * 25;
    let newY = this.ballPosition.y + this.velocity.y * 25;
    
    // Add gravity effect (downward pull)
    newY += Math.abs(this.velocity.x) * 2;
    
    // Apply boundaries and bouncing with energy conservation - use entire screen
    const maxX = window.innerWidth - 100;
    const maxY = window.innerHeight - 100;
    let bounced = false;
    
    // Bounce off walls with realistic energy loss
    if (newX < 0) {
      newX = Math.abs(newX);
      this.velocity.x *= -0.6;
      bounced = true;
    } else if (newX > maxX) {
      newX = maxX - (newX - maxX);
      this.velocity.x *= -0.6;
      bounced = true;
    }
    
    if (newY < 0) {
      newY = Math.abs(newY);
      this.velocity.y *= -0.6;
      bounced = true;
    } else if (newY > maxY) {
      newY = maxY - (newY - maxY);
      this.velocity.y *= -0.6;
      bounced = true;
    }
    
    this.ballPosition.x = Math.max(0, Math.min(maxX, newX));
    this.ballPosition.y = Math.max(0, Math.min(maxY, newY));
    
    // Add extra bounce effect if ball hit a wall
    if (bounced) {
      ball.style.filter = 'drop-shadow(0px 0px 30px rgba(255, 193, 7, 0.8)) brightness(1.3)';
      setTimeout(() => {
        ball.style.filter = '';
      }, 300);
    }
    
    setTimeout(() => {
      ball.classList.remove('throwing');
      this.bounceBall();
    }, 800);
  }

  private bounceBall(): void {
    this.isBouncing = true;
    setTimeout(() => {
      this.isBouncing = false;
    }, 1200);
  }

  private constrainBallPosition(): void {
    // Constrain to entire screen instead of just left container
    this.ballPosition.x = Math.max(0, Math.min(window.innerWidth - 100, this.ballPosition.x));
    this.ballPosition.y = Math.max(0, Math.min(window.innerHeight - 100, this.ballPosition.y));
  }

  showForgotPasswordModal = false;
  forgotEmail = '';

  openForgotPasswordModal() {
    this.forgotEmail = '';
    this.showForgotPasswordModal = true;
  }

  closeForgotPasswordModal() {
    this.showForgotPasswordModal = false;
  }

  async onLogin() {
    try {
      await this.authService.login({
        email: this.email,
        password: this.password,
      });

      // Redirect based on user role after successful login
      this.redirectBasedOnRole();
    } catch (error) {
      this.errorMessage = 'Incorrect username or password';
      console.error('Eroare la autentificare:', error);
    }
  }

  private redirectBasedOnRole() {
    const userRole = this.authService.getUserRole();

    // Redirect based on role
    if (userRole === UserRole.PLAYER) {
      this.router.navigate(['/player-dashboard']);
    } else if (userRole === UserRole.ORGANISER) {
      this.router.navigate(['/home']);
    } else {
      // Default fallback for ADMIN or unknown roles
      this.router.navigate(['/home']);
    }
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }

  forgotPassword() {
    this.email = this.forgotEmail;
    this.authService.forgotPassword(this.email).subscribe({
      next: () =>
        this.notificationService.showSuccess(
          'Email trimis cu succes! Verifică inbox-ul.'
        ),
      error: (err) => {
        console.error(err);
        this.errorMessage = 'A apărut o eroare la trimiterea email-ului.';
      },
    });
    this.forgotEmail = '';
    this.email = '';
    this.closeForgotPasswordModal();
  }
}
