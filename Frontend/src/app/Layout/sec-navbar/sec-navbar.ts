import { AfterViewInit, Component, ElementRef, ViewChild } from "@angular/core";

@Component({
  selector: "app-sec-navbar",
  standalone: true,
  templateUrl: "./sec-navbar.html",
  styleUrl: "./sec-navbar.css",
})
export class SecNavbar implements AfterViewInit {
  @ViewChild("mainNavbar") mainNavbar!: ElementRef;
  @ViewChild("heroSection") heroSection!: ElementRef;

  isScrolled = false;

  ngAfterViewInit(): void {
    this.adjustHeroPadding();
    this.updateNavbarStyle();

    window.addEventListener("scroll", () => {
      this.isScrolled = window.scrollY > 50;
      this.updateNavbarStyle();
    });
  }

  private adjustHeroPadding() {
    if (this.mainNavbar && this.heroSection) {
      const navbarHeight = this.mainNavbar.nativeElement.offsetHeight;
      this.heroSection.nativeElement.style.paddingTop = `${navbarHeight}px`;
    }
  }

  private updateNavbarStyle() {
    const navbar = this.mainNavbar?.nativeElement;
    if (!navbar) return;

    if (this.isScrolled) {
      navbar.classList.add("scrolled");
    } else {
      navbar.classList.remove("scrolled");
    }
  }
}
