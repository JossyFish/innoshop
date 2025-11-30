'use client'; 
import Link from "next/link";
import { usePathname } from "next/navigation";
import styles from  "./header.module.css";

const navItems = [
  { href: "/", label: "Products" },
  { href: "/profile", label: "Profile" }
];

export default function Header() {
  const pathname = usePathname();
  return (
      <header className={styles.header}>
      <Link href="/" className={styles.logo}>
        InnoShop
      </Link>
      
      <nav className={styles.nav}>
        {navItems.map((item) => (
          <Link 
            key={item.href} 
            href={item.href}
            className={`${styles.link} ${
              pathname === item.href ? styles.linkActive : ""
            }`}
          >
            {item.label}
          </Link>
        ))}
      </nav>
    </header>
  );
}