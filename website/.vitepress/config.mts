import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "EasyKit",
  description: "EasyKit Documentation - Powerful Windows toolkit for developers. Requires Microsoft .NET 8.0.",
  lang: "en-US",
  head: [
    // Site Metadata
    ['meta', { name: 'keywords', content: 'EasyKit, Windows toolkit, documentation, developer tools, productivity, Windows 10, Windows 11' }],
    ['meta', { name: 'author', content: 'EasyKit Team' }],
    // SEO: Contact Sources
    ['meta', { name: 'contact:github', content: 'https://github.com/LoveDoLove/EasyKit' }],
    ['meta', { name: 'contact:discord', content: 'https://discord.com/invite/FyYEmtRCRE' }],
    ['meta', { name: 'contact:telegram', content: 'https://t.me/lovedoloveofficialchannel' }],
    // SEO: Open Graph
    ['meta', { property: 'og:title', content: 'EasyKit Documentation' }],
    ['meta', { property: 'og:description', content: 'Comprehensive documentation for EasyKit, the Windows-only developer toolkit. Requires Microsoft .NET 8.0.' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:url', content: 'https://easykit.dev/' }],
    ['meta', { property: 'og:image', content: 'https://easykit.dev/images/icon.jpg' }],
    // SEO: Twitter Card
    ['meta', { name: 'twitter:card', content: 'summary_large_image' }],
    ['meta', { name: 'twitter:title', content: 'EasyKit Documentation' }],
    ['meta', { name: 'twitter:description', content: 'Comprehensive documentation for EasyKit, the Windows-only developer toolkit. Requires Microsoft .NET 8.0.' }],
    ['meta', { name: 'twitter:image', content: 'https://easykit.dev/images/icon.jpg' }],
    // SEO: Canonical URL
    ['link', { rel: 'canonical', href: 'https://easykit.dev/' }],
    // SEO: Sitemap
    ['link', { rel: 'sitemap', type: 'application/xml', href: '/sitemap.xml' }],
    // Accessibility: Skip to Content
    ['link', { rel: 'stylesheet', href: '/skip-to-content.css' }],
    // Performance: Preload critical assets
    ['link', { rel: 'preload', href: '/images/icon.jpg', as: 'image' }],
    // Performance: Enable caching
    ['meta', { 'http-equiv': 'Cache-Control', content: 'public, max-age=604800, immutable' }]
  ],
  themeConfig: {
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Features', link: '/features' },
      { text: 'Installation', link: '/installation' },
      { text: 'Usage', link: '/usage' },
      { text: 'FAQ', link: '/faq' },
      { text: 'Contact', link: '/contact' }
    ],
    sidebar: [
      {
        text: 'Documentation',
        items: [
          { text: 'Home', link: '/' },
          { text: 'Features', link: '/features' },
          { text: 'Installation', link: '/installation' },
          { text: 'Usage', link: '/usage' },
          { text: 'FAQ', link: '/faq' },
          { text: 'Contact', link: '/contact' }
        ]
      }
    ],
    socialLinks: [
      { icon: 'github', link: 'https://github.com/LoveDoLove/EasyKit', ariaLabel: 'Github' },
      { icon: 'discord', link: 'https://discord.com/invite/FyYEmtRCRE', ariaLabel: 'Discord' },
      { icon: 'telegram', link: 'https://t.me/lovedoloveofficialchannel', ariaLabel: 'Telegram' }
    ]
  }
})
