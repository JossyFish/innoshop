'use client'; 
import Link from "next/link";
import { usePathname } from "next/navigation";
import styles from  "./profileHeader.module.css";

const navItems = [
  { href: "/profile", label: "Home" },
  { href: "/profile/products", label: "Products" }
];

export const ProfileHeader = () => {
  const pathname = usePathname();
  return (
      <header className={styles.header}>
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